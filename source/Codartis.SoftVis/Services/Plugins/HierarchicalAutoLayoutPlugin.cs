using System.Collections.Concurrent;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Performs automatic layout for the root nodes and for all container nodes.
    /// </summary>
    /// <remarks>
    /// Responsibilities:
    /// Create and destroy layout engines for all container nodes.
    /// Route diagram events to the proper layout engine.
    /// </remarks>
    internal sealed class HierarchicalAutoLayoutPlugin : DiagramPluginBase
    {
        private readonly ILayoutPriorityProvider _layoutPriorityProvider;

        /// <summary>
        /// The root layout engine is for the root nodes.
        /// </summary>
        private IIncrementalLayoutEngine _rootLayoutEngine;

        ///// <summary>
        ///// The connector router that routes those connectors that connect nodes in different layout group.
        ///// </summary>
        //private IIncrementalLayoutEngine _crossLayoutGroupConnectorRouter;

        /// <summary>
        /// Holds a layout engine for each diagram node that can contain other nodes.
        /// </summary>
        private readonly IDictionary<ModelNodeId, IIncrementalLayoutEngine> _layoutEnginesPerNodes;

        /// <summary>
        /// Maps model nodes to their containing layout engine.
        /// </summary>
        private readonly IDictionary<ModelNodeId, IIncrementalLayoutEngine> _modelNodeToContainingLayoutEngine;

        /// <summary>
        /// Maps model relationships to their containing layout engine.
        /// </summary>
        private readonly IDictionary<ModelRelationshipId, IIncrementalLayoutEngine> _modelRelationshipToContainingLayoutEngine;

        public HierarchicalAutoLayoutPlugin(ILayoutPriorityProvider layoutPriorityProvider)
        {
            _layoutPriorityProvider = layoutPriorityProvider;
            _layoutEnginesPerNodes = new ConcurrentDictionary<ModelNodeId, IIncrementalLayoutEngine>();
            _modelNodeToContainingLayoutEngine = new ConcurrentDictionary<ModelNodeId, IIncrementalLayoutEngine>();
            _modelRelationshipToContainingLayoutEngine = new ConcurrentDictionary<ModelRelationshipId, IIncrementalLayoutEngine>();
        }

        public override void Initialize(IModelService modelService, IDiagramService diagramService)
        {
            base.Initialize(modelService, diagramService);

            //_crossLayoutGroupConnectorRouter = CrossLayoutGroupConnectorRouter(diagramService);
            _rootLayoutEngine = CreateLayoutEngine(diagramService);

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            DiagramService.DiagramChanged -= OnDiagramChanged;
        }

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            switch (diagramEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    var addedDiagramNode = diagramNodeAddedEvent.NewNode;
                    var addDiagramNodeAction = new AddDiagramNodeAction(addedDiagramNode);

                    if (addedDiagramNode is IContainerDiagramNode containerDiagramNode)
                    {
                        var incrementalLayoutEngine = CreateLayoutEngine(DiagramService);
                        _layoutEnginesPerNodes.Add(containerDiagramNode.Id, incrementalLayoutEngine);
                    }

                    var layoutEngineForAddedDiagramNode = GetLayoutEngine(addedDiagramNode);
                    layoutEngineForAddedDiagramNode.EnqueueDiagramAction(addDiagramNodeAction);
                    _modelNodeToContainingLayoutEngine.Add(addedDiagramNode.Id, layoutEngineForAddedDiagramNode);
                    break;

                case DiagramConnectorAddedEvent diagramConnectorAddedEvent:
                    var addedDiagramConnector = diagramConnectorAddedEvent.NewConnector;
                    var addDiagramConnectorAction = new AddDiagramConnectorAction(addedDiagramConnector);

                    var layoutEngineForAddedDiagramConnector = GetLayoutEngine(addedDiagramConnector);
                    if (layoutEngineForAddedDiagramConnector != null)
                    {
                        layoutEngineForAddedDiagramConnector.EnqueueDiagramAction(addDiagramConnectorAction);
                        _modelRelationshipToContainingLayoutEngine.Add(addedDiagramConnector.Id, layoutEngineForAddedDiagramConnector);
                    }
                    break;

                case DiagramNodeSizeChangedEvent diagramNodeSizeChangedEvent:
                    var resizedDiagramNode = diagramNodeSizeChangedEvent.NewNode;
                    var resizeDiagramNodeAction = new ResizeDiagramNodeAction(resizedDiagramNode, resizedDiagramNode.Size);
                    _modelNodeToContainingLayoutEngine[resizedDiagramNode.Id].EnqueueDiagramAction(resizeDiagramNodeAction);
                    break;

                case DiagramNodeRemovedEvent diagramNodeRemovedEvent:
                    var removedDiagramNode = diagramNodeRemovedEvent.OldNode;
                    var removeDiagramNodeAction = new RemoveDiagramNodeAction(removedDiagramNode);
                    _modelNodeToContainingLayoutEngine[removedDiagramNode.Id].EnqueueDiagramAction(removeDiagramNodeAction);
                    break;

                case DiagramConnectorRemovedEvent diagramConnectorRemovedEvent:
                    var removedDiagramConnector = diagramConnectorRemovedEvent.OldConnector;
                    var removeDiagramConnectorAction = new RemoveDiagramConnectorAction(removedDiagramConnector);
                    _modelRelationshipToContainingLayoutEngine[removedDiagramConnector.Id].EnqueueDiagramAction(removeDiagramConnectorAction);
                    break;

                case DiagramClearedEvent _:
                    foreach (var incrementalLayoutEngine in _layoutEnginesPerNodes.Values)
                        incrementalLayoutEngine.Dispose();

                    _layoutEnginesPerNodes.Clear();
                    _rootLayoutEngine.EnqueueDiagramAction(new ClearDiagramAction());
                    break;
            }
        }

        private IncrementalLayoutEngine CreateLayoutEngine(IDiagramService diagramService)
        {
            var layoutCalculator = new IncrementalLayoutCalculator(_layoutPriorityProvider);
            return new IncrementalLayoutEngine(layoutCalculator, diagramService);
        }

        private IIncrementalLayoutEngine GetLayoutEngine(IDiagramConnector diagramConnector)
        {
            return diagramConnector.Source.ParentNodeId  != null &&
                   diagramConnector.Source.ParentNodeId == diagramConnector.Target.ParentNodeId
                ? _layoutEnginesPerNodes[diagramConnector.Source.ParentNodeId.Value]
                : null; // TODO: _crossLayoutGroupConnectorRouter
        }

        private IIncrementalLayoutEngine GetLayoutEngine(IDiagramNode diagramNode)
        {
            return diagramNode.ParentNodeId != null
                ? _layoutEnginesPerNodes[diagramNode.ParentNodeId.Value]
                : _rootLayoutEngine;
        }
    }
}