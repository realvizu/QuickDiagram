using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public sealed class PayloadWrapperRelatedNodeItemViewModel : IRelatedNodeItemViewModel
    {
        private readonly IModelNode _modelNode;

        public PayloadWrapperRelatedNodeItemViewModel([NotNull] IModelNode modelNode)
        {
            _modelNode = modelNode;
        }

        public ModelNodeId Id => _modelNode.Id;
        public object Payload => _modelNode.Payload;

        public override string ToString() => _modelNode.ToString();
    }
}