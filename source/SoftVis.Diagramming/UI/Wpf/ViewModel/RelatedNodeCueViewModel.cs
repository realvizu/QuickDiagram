using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a visual cue that indicates the availability of related nodes that are not on the diagram yet.
    /// </summary>
    public class RelatedNodeCueViewModel : DiagramShapeDecoratorViewModelBase
    {
        private readonly IDiagramNode _diagramNode;
        private readonly DirectedModelRelationshipType _directedModelRelationshipType;

        private IModel _lastModel;
        private IDiagram _lastDiagram;

        public RelatedNodeCueViewModel(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore,
            IDiagramNode diagramNode, RelatedNodeType relatedNodeType)
            : base(modelStore, diagramStore)
        {
            _diagramNode = diagramNode;
            _directedModelRelationshipType = relatedNodeType.RelationshipType;

            _lastModel = modelStore.CurrentModel;
            _lastDiagram = diagramStore.CurrentDiagram;

            ModelStore.ModelChanged += OnModelChanged;
            DiagramStore.DiagramChanged += OnDiagramChanged;

            UpdateVisibility();
        }

        public override  void Dispose()
        {
            base.Dispose();

            ModelStore.ModelChanged -= OnModelChanged;
            DiagramStore.DiagramChanged -= OnDiagramChanged;
        }

        public override object PlacementKey => _directedModelRelationshipType;
        
        private void OnModelChanged(ModelEventBase modelEvent)
        {
            _lastModel = modelEvent.NewModel;
            UpdateVisibility();
        }

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            _lastDiagram = diagramEvent.NewDiagram;
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            IsVisible = GetUndisplayedRelatedModelNodes(_diagramNode.ModelNode).Any();
        }

        private IEnumerable<IModelNode> GetUndisplayedRelatedModelNodes(IModelNode modelNode)
        {
            return _lastModel.GetRelatedNodes(modelNode, _directedModelRelationshipType)
                .Except(_lastDiagram.Nodes.Select(j => j.ModelNode));
        }
    }
}
