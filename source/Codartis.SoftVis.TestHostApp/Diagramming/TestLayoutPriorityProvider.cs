using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    /// <summary>
    /// Assigns layout priority to diagram nodes.
    /// </summary>
    internal class TestLayoutPriorityProvider : ILayoutPriorityProvider
    {
        public int GetPriority(IDiagramNode diagramNode)
        {
            var nodeType = (diagramNode as TypeDiagramNode)?.TypeNode.Stereotype;

            if (nodeType == ModelNodeStereotypes.Class) return 2;
            if (nodeType == ModelNodeStereotypes.Interface) return 1;
            return 0;
        }
    }
}
