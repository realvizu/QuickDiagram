using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Assigns layout priority to diagram nodes.
    /// </summary>
    internal class RoslynLayoutPriorityProvider : ILayoutPriorityProvider
    {
        public int GetPriority(IDiagramNode diagramNode)
        {
            var nodeType = (diagramNode as RoslynTypeDiagramNode)?.Stereotype;

            if (nodeType == ModelNodeStereotypes.Class) return 2;
            if (nodeType == ModelNodeStereotypes.Interface) return 1;
            return 0;
        }
    }
}
