using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public sealed class PayloadWrapperRelatedNodeItemViewModelFactory : IRelatedNodeItemViewModelFactory
    {
        public IRelatedNodeItemViewModel Create(IModelNode modelNode)
        {
            return new PayloadWrapperRelatedNodeItemViewModel(modelNode);
        }
    }
}