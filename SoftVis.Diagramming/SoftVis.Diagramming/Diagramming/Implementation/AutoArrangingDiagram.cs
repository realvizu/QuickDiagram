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

        public AutoArrangingDiagram(IReadOnlyModel model)
            : base(model)
        {
            _incrementalLayoutEngine = new IncrementalLayoutEngine();
            _layoutActionExecutor = new LayoutActionExecutorVisitor(this);
            _automaticLayoutCancellation = new CancellationTokenSource();
            _diagramActionQueue = new Queue<DiagramAction>();
            _diagramActionArrivedEvent = new AutoResetEvent(false);

            ShapeAdded += OnShapeAdded;
            ShapeRemoved += OnShapeRemoved;
            NodeSizeChanged += OnNodeSizeChanged;
            BatchAddStarted += OnBatchStarted;
            BatchAddFinished += OnBatchFinished;
            BatchRemoveStarted += OnBatchStarted;
            BatchRemoveFinished += OnBatchFinished;

            Task.Run(() => ProcessDiagramShapeActions(_automaticLayoutCancellation.Token));
        }

        public void Dispose()
        {
            _automaticLayoutCancellation.Cancel();
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
                EnqueueDiagramAction(new DiagramNodeAction(diagramNode, ShapeActionType.Add));

            var diagramConnector = diagramShape as DiagramConnector;
            if (diagramConnector != null)
                EnqueueDiagramAction(new DiagramConnectorAction(diagramConnector, ShapeActionType.Add));
        }

        private void OnNodeSizeChanged(IDiagramNode diagramNode, Size2D oldSize, Size2D newSize)
        {
            EnqueueDiagramAction(new DiagramNodeAction(diagramNode, ShapeActionType.Resize));
        }

        private void OnShapeRemoved(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as DiagramNode;
            if (diagramNode != null)
                EnqueueDiagramAction(new DiagramNodeAction(diagramNode, ShapeActionType.Remove));

            var diagramConnector = diagramShape as DiagramConnector;
            if (diagramConnector != null)
                EnqueueDiagramAction(new DiagramConnectorAction(diagramConnector, ShapeActionType.Remove));
        }

        private void OnBatchStarted()
        {
            EnqueueDiagramAction(new DiagramBatchAction(BatchActionType.Start));
        }

        private void OnBatchFinished()
        {
            EnqueueDiagramAction(new DiagramBatchAction(BatchActionType.Finish));
        }

        private void ProcessDiagramShapeActions(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_diagramActionArrivedEvent.WaitOne(TimeSpan.FromSeconds(1)) && NotInBatch(_diagramActionQueue))
                {
                    var diagramActions = GetBatchFromQueue(_diagramActionQueue);
                    if (diagramActions.Any())
                        ApplyDiagramActions(diagramActions);
                }
            }
        }

        private static bool NotInBatch(Queue<DiagramAction> diagramActionQueue)
        {
            lock (diagramActionQueue)
            {
                return diagramActionQueue.All(i => !IsBatchStart(i))
                     || diagramActionQueue.Any(IsBatchFinish);
            }
        }

        private static bool IsBatchStart(DiagramAction diagramAction) =>
            diagramAction is DiagramBatchAction && ((DiagramBatchAction)diagramAction).Type == BatchActionType.Start;

        private static bool IsBatchFinish(DiagramAction diagramAction) =>
            diagramAction is DiagramBatchAction && ((DiagramBatchAction)diagramAction).Type == BatchActionType.Finish;

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
            var layoutActions = _incrementalLayoutEngine.CalculateLayoutActions(diagramActions).ToList();
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
