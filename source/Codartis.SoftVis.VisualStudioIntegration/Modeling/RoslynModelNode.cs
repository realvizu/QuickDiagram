using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    public struct RoslynModelNode
    {
        [NotNull] public IModelNode ModelNode { get; }

        public RoslynModelNode([NotNull] IModelNode modelNode)
        {
            ModelNode = modelNode;
        }

        public ModelNodeId Id => ModelNode.Id;
        public ModelNodeStereotype Stereotype => ModelNode.Stereotype;
        public IRoslynSymbol RoslynSymbol => (IRoslynSymbol)ModelNode.Payload;
    }
}