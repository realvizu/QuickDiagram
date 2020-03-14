using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public interface IRelatedNodeItemViewModelFactory
    {
        [NotNull]
        IRelatedNodeItemViewModel Create([NotNull] IModelNode modelNode);
    }
}