using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Builds a diagram from model elements.
    /// </summary>
    public class RoslynBasedDiagramBuilder
    {
        private readonly Diagram _diagram;

        internal RoslynBasedDiagramBuilder(Diagram diagram)
        {
            _diagram = diagram;
        }

        public void ShowModelEntity(IModelEntity modelEntity)
        {
            if (modelEntity is RoslynBasedClass)
            {
                ShowClassWithBaseAndChildren((RoslynBasedClass)modelEntity);
                UpdateLayout();
            }
        }

        private void ShowClassWithBaseAndChildren(RoslynBasedClass @class)
        {
            ShowBase(@class);
            ShowClass(@class);
            ShowChildren(@class);
        }

        private void ShowChildren(RoslynBasedClass @class)
        {
            foreach (var childClass in @class.ChildClasses)
            {
                ShowClass(childClass);
                ShowChildren(childClass);
            }
        }

        private void ShowBase(RoslynBasedClass @class)
        {
            if (@class.BaseClass != null)
            {
                ShowBase(@class.BaseClass);
                ShowClass(@class.BaseClass);
            }
        }

        private void ShowClass(RoslynBasedClass @class)
        {
            _diagram.ShowNode(@class);
        }

        private void UpdateLayout()
        {
            _diagram.LayoutNodes();
            _diagram.RouteConnectors();
        }
    }
}