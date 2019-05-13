//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Codartis.SoftVis.Diagramming;
//using Codartis.SoftVis.Diagramming.Events;
//using Codartis.SoftVis.Diagramming.Layout;
//using Codartis.SoftVis.Diagramming.Layout.Incremental;
//using Codartis.SoftVis.Modeling;

//namespace Codartis.SoftVis.Services.Plugins
//{
//    /// <summary>
//    /// Performs automatic layout for the root nodes and for all container nodes.
//    /// </summary>
//    /// <remarks>
//    /// Responsibilities:
//    /// Create and destroy layout engines for all container nodes.
//    /// Route diagram events to the proper layout engine.
//    /// </remarks>
//    internal sealed class HierarchicalAutoLayoutPlugin : DiagramPluginBase
//    {
//        /// <summary>
//        /// The root layout engine is for the root nodes.
//        /// </summary>
//        private readonly IIncrementalLayoutCalculator _rootLayoutCalculator;

//        /// <summary>
//        /// Holds a layout engine for each diagram node that can contain other nodes.
//        /// </summary>
//        private readonly IDictionary<IContainerDiagramNode, IIncrementalLayoutCalculator> _layoutEnginesPerNodes;
        
//        public HierarchicalAutoLayoutPlugin(ILayoutPriorityProvider layoutPriorityProvider)
//        {
//            _rootLayoutCalculator = new IncrementalLayoutCalculator(layoutPriorityProvider);
//            _layoutEnginesPerNodes = new ConcurrentDictionary<IContainerDiagramNode, IIncrementalLayoutCalculator>();
//        }

//        public override void Initialize(IModelService modelService, IDiagramService diagramService)
//        {
//            base.Initialize(modelService, diagramService);

//            DiagramService.DiagramChanged += OnDiagramChanged;
//        }

//        public override void Dispose()
//        {
//            DiagramService.DiagramChanged -= OnDiagramChanged;
//        }

//        private void OnDiagramChanged(DiagramEventBase diagramEvent)
//        {
//            switch (diagramEvent)
//            {
//                case DiagramNodeAddedEvent diagramNodeAddedEvent:
//                    // Layout only top level nodes (with no parent diagram node)
//                    if (diagramNodeAddedEvent.DiagramNode.ParentDiagramNode == null)
//                        EnqueueDiagramAction(new AddDiagramNodeAction(diagramNodeAddedEvent.DiagramNode));
//                    break;

//                case DiagramConnectorAddedEvent diagramConnectorAddedEvent:
//                    EnqueueDiagramAction(new AddDiagramConnectorAction(diagramConnectorAddedEvent.DiagramConnector));
//                    break;

//                case DiagramNodeSizeChangedEvent diagramNodeSizeChangedEvent:
//                    var diagramNode = diagramNodeSizeChangedEvent.DiagramNode;
//                    EnqueueDiagramAction(new ResizeDiagramNodeAction(diagramNode, diagramNode.Size));
//                    break;

//                case DiagramNodeRemovedEvent diagramNodeRemovedEvent:
//                    EnqueueDiagramAction(new RemoveDiagramNodeAction(diagramNodeRemovedEvent.DiagramNode));
//                    break;

//                case DiagramConnectorRemovedEvent diagramConnectorRemovedEvent:
//                    EnqueueDiagramAction(new RemoveDiagramConnectorAction(diagramConnectorRemovedEvent.DiagramConnector));
//                    break;

//                case DiagramClearedEvent _:
//                    EnqueueDiagramAction(new ClearDiagramAction());
//                    break;
//            }
//        }
//    }
//}
