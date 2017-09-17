﻿using System;
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

        public RelatedNodeCueViewModel(IModelService modelService, IDiagramService diagramService,
            IDiagramNode diagramNode, RelatedNodeType relatedNodeType)
            : base(modelService, diagramService)
        {
            _diagramNode = diagramNode;
            _directedModelRelationshipType = relatedNodeType.RelationshipType;

            _lastModel = modelService.Model;
            _lastDiagram = diagramService.Diagram;

            ModelService.ModelChanged += OnModelChanged;
            DiagramService.DiagramChanged += OnDiagramChanged;

            UpdateVisibility();
        }

        public override  void Dispose()
        {
            base.Dispose();

            ModelService.ModelChanged -= OnModelChanged;
            DiagramService.DiagramChanged -= OnDiagramChanged;
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
            return _lastModel.GetRelatedNodes(modelNode.Id, _directedModelRelationshipType)
                .Except(_lastDiagram.Nodes.Select(j => j.ModelNode));
        }
    }
}
