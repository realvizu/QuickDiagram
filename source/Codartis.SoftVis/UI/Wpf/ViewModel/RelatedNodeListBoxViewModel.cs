using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using Codartis.Util.UI.Wpf.ViewModels;

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

        public IDiagramShapeUi OwnerDiagramShape => _ownerButton?.HostViewModel;

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

        private void OnModelChanged(ModelEvent modelEvent)
        {
            foreach (var itemChange in modelEvent.ItemEvents)
                ProcessModelItemEvent(itemChange);
        }

        private void ProcessModelItemEvent(ModelItemEventBase modelEvent)
        {
            if (modelEvent is ModelNodeRemovedEvent modelNodeRemovedEvent)
                RemoveModelNode(modelNodeRemovedEvent.RemovedNode.Id);
        }

        private void OnDiagramChanged(DiagramEvent @event)
        {
            foreach (var diagramChange in @event.ShapeEvents)
                ProcessDiagramChange(diagramChange);
        }

        private void ProcessDiagramChange(DiagramShapeEventBase diagramShapeEvent)
        {
            if (diagramShapeEvent is DiagramNodeAddedEvent diagramNodeAddedEvent)
                RemoveModelNode(diagramNodeAddedEvent.NewNode.Id);
        }

        private void RemoveModelNode(ModelNodeId modelNodeId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.Id == modelNodeId);
            if (itemToRemove != null)
                Items.Remove(itemToRemove);
        }
    }
}