using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a bubble list box that attaches to related entity selector diagram shape buttons.
    /// </summary>
    public class RelatedEntityListBoxViewModel : BubbleListBoxViewModel<IModelEntity>
    {
        private ShowRelatedNodeButtonViewModel _ownerButton;

        public ShowRelatedNodeButtonViewModel OwnerButton
        {
            get { return _ownerButton; }
            set
            {
                _ownerButton = value;
                OnPropertyChanged();
            }
        }

        public DiagramShapeViewModelBase OwnerDiagramShape => _ownerButton?.AssociatedDiagramShapeViewModel;

        public void Show(ShowRelatedNodeButtonViewModel ownerButton, IEnumerable<IModelEntity> items)
        {
            base.Show(items);
            OwnerButton = ownerButton;
        }

        public override void Hide()
        {
            OwnerButton = null;
            base.Hide();
        }
    }
}
