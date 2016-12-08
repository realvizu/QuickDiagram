using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a bubble list box that attaches to related entity selector diagram shape buttons.
    /// </summary>
    public class RelatedEntityListBoxViewModel : BubbleListBoxViewModel<IModelEntity>, IDisposable
    {
        private readonly IDiagram _diagram;

        public RelatedEntityListBoxViewModel(IDiagram diagram)
        {
            _diagram = diagram;
            _diagram.ShapeAdded += OnDiagramShapeAdded;
        }

        public void Dispose()
        {
            _diagram.ShapeAdded -= OnDiagramShapeAdded;
        }

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

        public DiagramShapeViewModelBase OwnerDiagramShape => _ownerButton?.HostViewModel;

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

        private void OnDiagramShapeAdded(IDiagramShape diagramShape)
        {
            var removedModelEntity = diagramShape?.ModelItem as IModelEntity;
            if (removedModelEntity == null)
                return;

            Items.Remove(removedModelEntity);
        }
    }
}
