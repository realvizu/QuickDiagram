using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Service.Plugins
{
    /// <summary>
    /// A diagram that maintains it own layout.
    /// Responds to shape addition/removal and uses a layout engine that calculates how to arrange nodes and connectors.
    /// </summary>
    public class AutoLayoutDiagramPlugin : DiagramPluginBase
    {
        private readonly IIncrementalLayoutEngine _incrementalLayoutEngine;
        private LayoutActionExecutorVisitor _layoutActionExecutor;
        private CancellationTokenSource _automaticLayoutCancellation;
        private Queue<DiagramAction> _diagramActionQueue;
        private AutoResetEvent _diagramActionArrivedEvent;

        public AutoLayoutDiagramPlugin(ILayoutPriorityProvider layoutPriorityProvider)
        {
            _incrementalLayoutEngine = new IncrementalLayoutEngine(layoutPriorityProvider);
        }

        public override void Initialize(IModelService modelService, IDiagramService diagramService)
        {
            base.Initialize(modelService, diagramService);

            _layoutActionExecutor = new LayoutActionExecutorVisitor(DiagramService);
            _automaticLayoutCancellation = new CancellationTokenSource();
            _diagramActionQueue = new Queue<DiagramAction>();
            _diagramActionArrivedEvent = new AutoResetEvent(false);

            DiagramService.DiagramChanged += OnDiagramChanged;

            Task.Run(() => ProcessDiagramShapeActionsAsync(_automaticLayoutCancellation.Token));
        }

        public override void Dispose()
        {
            DiagramService.DiagramChanged -= OnDiagramChanged;

            _automaticLayoutCancellation.Cancel();
            _automaticLayoutCancellation.Dispose();
        }

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            switch (diagramEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    EnqueueDiagramAction(new AddDiagramNodeAction(diagramNodeAddedEvent.DiagramNode));
                    break;

                case DiagramConnectorAddedEvent diagramConnectorAddedEvent:
                    EnqueueDiagramAction(new AddDiagramConnectorAction(diagramConnectorAddedEvent.DiagramConnector));
                    break;

                case DiagramNodeSizeChangedEvent diagramNodeSizeChangedEvent:
                    var diagramNode = diagramNodeSizeChangedEvent.DiagramNode;
                    EnqueueDiagramAction(new ResizeDiagramNodeAction(diagramNode, diagramNode.Size));
                    break;

                case DiagramNodeRemovedEvent diagramNodeRemovedEvent:
                    EnqueueDiagramAction(new RemoveDiagramNodeAction(diagramNodeRemovedEvent.DiagramNode));
                    break;

                case DiagramConnectorRemovedEvent diagramConnectorRemovedEvent:
                    EnqueueDiagramAction(new RemoveDiagramConnectorAction(diagramConnectorRemovedEvent.DiagramConnector));
                    break;

                case DiagramClearedEvent _:
                    _incrementalLayoutEngine.Clear();
                    break;
            }
        }

        private void EnqueueDiagramAction(DiagramAction diagramAction)
        {
            lock (_diagramActionQueue)
            {
                _diagramActionQueue.Enqueue(diagramAction);
            }
            _diagramActionArrivedEvent.Set();
        }

        private async void ProcessDiagramShapeActionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_diagramActionArrivedEvent.WaitOne(TimeSpan.FromSeconds(1)))
                {
                    while (await AreEventsArrivingInRapidSuccessionAsync())
                    { }

                    var diagramActions = GetBatchFromQueue(_diagramActionQueue);
                    if (diagramActions.Any())
                        ApplyDiagramActions(diagramActions);
                }
            }
        }

        private async Task<bool> AreEventsArrivingInRapidSuccessionAsync()
        {
            var queueLengthBeforeWait = GetDiagramActionQueueLength();
            await Task.Delay(TimeSpan.FromMilliseconds(5));
            var queueLengthAfterWait = GetDiagramActionQueueLength();

            return queueLengthAfterWait > queueLengthBeforeWait;
        }

        private int GetDiagramActionQueueLength()
        {
            lock (_diagramActionQueue)
                return _diagramActionQueue.Count;
        }

        private static List<DiagramAction> GetBatchFromQueue(Queue<DiagramAction> diagramActionQueue)
        {
            lock (diagramActionQueue)
            {
                var result = diagramActionQueue.ToList();
                diagramActionQueue.Clear();
                return result;
            }
        }

        private void ApplyDiagramActions(List<DiagramAction> diagramActions)
        {
            //Debug.WriteLine($"{DateTime.Now:O} | ApplyDiagramActions");
            //foreach (var diagramAction in diagramActions)
            //    Debug.WriteLine($"  {diagramAction}");

            var layoutActions = _incrementalLayoutEngine.CalculateLayoutActions(diagramActions).ToList();

            //Debug.WriteLine($"{DateTime.Now:O} | ApplyLayoutActions");
            //foreach (var layoutAction in layoutActions)
            //    Debug.WriteLine($"  {layoutAction}");

            ApplyLayoutActionsToDiagram(layoutActions);
        }

        private void ApplyLayoutActionsToDiagram(IEnumerable<ILayoutAction> layoutActions)
        {
            var netLayoutActions = CombineLayoutAction(layoutActions);
            foreach (var layoutAction in netLayoutActions)
                layoutAction.AcceptVisitor(_layoutActionExecutor);
        }

        private static IEnumerable<ILayoutAction> CombineLayoutAction(IEnumerable<ILayoutAction> layoutActions)
        {
            return layoutActions.GroupBy(i => i.DiagramShape).Select(j => j.Last());
        }
    }
}
