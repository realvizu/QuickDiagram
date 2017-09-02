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
        private readonly IModelService _modelService;
        private readonly IDiagramService _diagramService;

        public RelatedNodeListBoxViewModel(IModelService modelService, IDiagramService diagramService)
        {
            _modelService = modelService;
            _modelService.ModelChanged += OnModelChanged;

            _diagramService = diagramService;
            _diagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            _modelService.ModelChanged -= OnModelChanged;
            _diagramService.DiagramChanged -= OnDiagramChanged;
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
