using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RelatedNodeItemViewModel : ViewModelBase, IRelatedNodeItemViewModel, ICommonRoslynNodeViewModel
    {
        public ModelNodeId Id { get; }
        public string Name { get; }
        public string FullName { get; }
        public ModelNodeStereotype Stereotype { get; }
        public bool IsStatic { get; }

        public RelatedNodeItemViewModel(
            ModelNodeId id,
            string name,
            string fullName,
            ModelNodeStereotype stereotype,
            bool isStatic)
        {
            Id = id;
            Name = name;
            FullName = fullName;
            Stereotype = stereotype;
            IsStatic = isStatic;
        }
    }
}