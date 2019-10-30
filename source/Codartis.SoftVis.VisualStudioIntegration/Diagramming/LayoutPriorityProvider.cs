using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Assigns layout priority to diagram nodes.
    /// </summary>
    internal class LayoutPriorityProvider : ILayoutPriorityProvider
    {
        public int GetPriority(IDiagramNode diagramNode)
        {
            var nodeType = diagramNode.ModelNode.Stereotype;

            if (nodeType == ModelNodeStereotypes.Class) return 2;
            if (nodeType == ModelNodeStereotypes.Interface) return 1;
            return 0;
        }
    }
}
