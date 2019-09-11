using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
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
            var nodeType = diagramNode.ModelNode.Stereotype;

            if (nodeType.Equals(ModelNodeStereotypes.Class)) return 2;
            if (nodeType.Equals(ModelNodeStereotypes.Interface)) return 1;
            return 0;
        }
    }
}
