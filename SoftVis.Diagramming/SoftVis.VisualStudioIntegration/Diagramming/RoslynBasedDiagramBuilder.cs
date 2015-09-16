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
                ShowClassWithRelatedEntities((RoslynBasedClass)modelEntity);
                UpdateLayout();
            }
            else if (modelEntity is RoslynBasedInterface)
            {
                ShowInterfaceWithRelatedEntities((RoslynBasedInterface)modelEntity);
                UpdateLayout();
            }
            else if (modelEntity is RoslynBasedStruct)
            {
                ShowStructWithRelatedEntities((RoslynBasedStruct)modelEntity);
                UpdateLayout();
            }
        }

        private void ShowClassWithRelatedEntities(RoslynBasedClass @class)
        {
            _diagram.ShowNode(@class);
            ShowBaseClass(@class);
            ShowImplementedInterfaces(@class);
            ShowDerivedClasses(@class);
        }

        private void ShowInterfaceWithRelatedEntities(RoslynBasedInterface @interface)
        {
            _diagram.ShowNode(@interface);
            ShowBaseInterfaces(@interface);
            ShowDerivedInterfaces(@interface);
            ShowImplementers(@interface);
        }

        private void ShowStructWithRelatedEntities(RoslynBasedStruct @struct)
        {
            _diagram.ShowNode(@struct);
            ShowImplementedInterfaces(@struct);
        }

        private void ShowImplementedInterfaces(dynamic @classOrStruct)
        {
            foreach (var @interface in @classOrStruct.ImplementedInterfaces)
            {
                _diagram.ShowNode(@interface);
                ShowBaseInterfaces(@interface);
            }
        }

        private void ShowDerivedClasses(RoslynBasedClass @class)
        {
            foreach (var childClass in @class.DerivedClasses)
            {
                _diagram.ShowNode(childClass);
                ShowDerivedClasses(childClass);
            }
        }

        private void ShowDerivedInterfaces(RoslynBasedInterface @interface)
        {
            foreach (var @derivedInterface in @interface.DerivedInterfaces)
            {
                _diagram.ShowNode(@derivedInterface);
                ShowDerivedInterfaces(@derivedInterface);
            }
        }

        private void ShowImplementers(RoslynBasedInterface @interface)
        {
            foreach (var @class in @interface.ImplementerTypes)
            {
                _diagram.ShowNode(@class);
            }
        }

        private void ShowBaseClass(RoslynBasedClass @class)
        {
            if (@class.BaseClass != null)
            {
                _diagram.ShowNode(@class.BaseClass);
                ShowBaseClass(@class.BaseClass);
            }
        }

        private void ShowBaseInterfaces(RoslynBasedInterface @interface)
        {
            foreach (var baseInterface in @interface.BaseInterfaces)
            {
                _diagram.ShowNode(baseInterface);
                ShowBaseInterfaces(baseInterface);
            }
        }

        private void UpdateLayout()
        {
            _diagram.LayoutNodes();
            _diagram.RouteConnectors();
        }
    }
}