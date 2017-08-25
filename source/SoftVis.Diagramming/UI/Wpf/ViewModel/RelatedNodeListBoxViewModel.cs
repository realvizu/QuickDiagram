using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Events;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a bubble list box that attaches to related entity selector diagram shape buttons.
    /// </summary>
    public class RelatedNodeListBoxViewModel : BubbleListBoxViewModel<IModelNode>
    {
        private readonly IReadOnlyModelStore _modelStore;
        private readonly IReadOnlyDiagramStore _diagramStore;

        public RelatedNodeListBoxViewModel(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore)
        {
            _modelStore = modelStore;
            _modelStore.ModelChanged += OnModelChanged;

            _diagramStore = diagramStore;
            _diagramStore.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            _modelStore.ModelChanged -= OnModelChanged;
            _diagramStore.DiagramChanged -= OnDiagramChanged;
        }

        private RelatedNodeMiniButtonViewModel _ownerButton;

        public RelatedNodeMiniButtonViewModel OwnerButton
        {
            get { return _ownerButton; }
            set
            {
                _ownerButton = value;
                OnPropertyChanged();
            }
        }

        public DiagramShapeViewModelBase OwnerDiagramShape => _ownerButton?.HostViewModel;

        public void Show(RelatedNodeMiniButtonViewModel ownerButton, IEnumerable<IModelNode> items)
        {
            OwnerButton = ownerButton;
            base.Show(items);
        }

        public override void Hide()
        {
            OwnerButton = null;
            base.Hide();
        }

        private void OnModelChanged(ModelEventBase modelEvent)
        {
            if (modelEvent is ModelNodeRemovedEvent modelNodeRemovedEvent)
                RemoveModelNode(modelNodeRemovedEvent.RemovedNode.Id);
        }

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            if (diagramEvent is DiagramNodeAddedEvent diagramNodeAddedEvent)
                RemoveModelNode(diagramNodeAddedEvent.DiagramNode.Id);
        }

        private void RemoveModelNode(ModelNodeId modelNodeId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.Id == modelNodeId);
            if (itemToRemove != null)
                Items.Remove(itemToRemove);
        }
    }
}
