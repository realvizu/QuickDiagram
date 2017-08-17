using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A diagram that maintains it own layout.
    /// Responds to shape addition/removal and uses a layout engine that calculates how to arrange nodes and connectors.
    /// </summary>
    public class AutoArrangingDiagram : Diagram, IDisposable
    {
        private readonly IIncrementalLayoutEngine _incrementalLayoutEngine;
        private readonly LayoutActionExecutorVisitor _layoutActionExecutor;
        private readonly CancellationTokenSource _automaticLayoutCancellation;
        private readonly Queue<DiagramAction> _diagramActionQueue;
        private readonly AutoResetEvent _diagramActionArrivedEvent;

        public AutoArrangingDiagram(IModelBuilder modelBuilder, DiagramBuilder diagramBuilder, IDiagramNodeFactory diagramNodeFactory)
            : base(modelBuilder, diagramBuilder, diagramNodeFactory)
        {
            _incrementalLayoutEngine = new IncrementalLayoutEngine();
            _layoutActionExecutor = new LayoutActionExecutorVisitor(this);
            _automaticLayoutCancellation = new CancellationTokenSource();
            _diagramActionQueue = new Queue<DiagramAction>();
            _diagramActionArrivedEvent = new AutoResetEvent(false);

            ShapeAdded += OnShapeAdded;
            ShapeRemoved += OnShapeRemoved;
            NodeSizeChanged += OnNodeSizeChanged;

            Task.Run(() => ProcessDiagramShapeActionsAsync(_automaticLayoutCancellation.Token));
        }

        public void Dispose()
        {
            _automaticLayoutCancellation.Cancel();
            _automaticLayoutCancellation.Dispose();
        }

        private void EnqueueDiagramAction(DiagramAction diagramAction)
        {
            lock (_diagramActionQueue)
            {
                _diagramActionQueue.Enqueue(diagramAction);
            }
            _diagramActionArrivedEvent.Set();
        }

        public override void Clear()
        {
            _incrementalLayoutEngine.Clear();
            base.Clear();
        }

        private void OnShapeAdded(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as DiagramNode;
            if (diagramNode != null)
                EnqueueDiagramAction(new AddDiagramNodeAction(diagramNode));

            var diagramConnector = diagramShape as DiagramConnector;
            if (diagramConnector != null)
                EnqueueDiagramAction(new AddDiagramConnectorAction(diagramConnector));
        }

        private void OnNodeSizeChanged(IDiagramNode diagramNode, Size2D oldSize, Size2D newSize)
        {
            EnqueueDiagramAction(new ResizeDiagramNodeAction(diagramNode, newSize));
        }

        private void OnShapeRemoved(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as DiagramNode;
            if (diagramNode != null)
                EnqueueDiagramAction(new RemoveDiagramNodeAction(diagramNode));

            var diagramConnector = diagramShape as DiagramConnector;
            if (diagramConnector != null)
                EnqueueDiagramAction(new RemoveDiagramConnectorAction(diagramConnector));
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
