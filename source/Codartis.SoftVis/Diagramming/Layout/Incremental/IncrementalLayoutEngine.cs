using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Wraps an incremental layout calculator and provides a queue interface.
    /// </summary>
    internal class IncrementalLayoutEngine : IIncrementalLayoutEngine
    {
        private readonly IIncrementalLayoutCalculator _incrementalLayoutCalculator;
        private readonly LayoutActionExecutorVisitor _layoutActionExecutor;
        private readonly CancellationTokenSource _layoutEngineCancellation;
        private readonly Queue<DiagramAction> _diagramActionQueue;
        private readonly AutoResetEvent _diagramActionArrivedEvent;

        public IncrementalLayoutEngine(ILayoutPriorityProvider layoutPriorityProvider, IDiagramService diagramService)
        {
            _incrementalLayoutCalculator = new IncrementalLayoutCalculator(layoutPriorityProvider);

            _layoutActionExecutor = new LayoutActionExecutorVisitor(diagramService);
            _layoutEngineCancellation = new CancellationTokenSource();
            _diagramActionQueue = new Queue<DiagramAction>();
            _diagramActionArrivedEvent = new AutoResetEvent(false);

            // BUGBUG: is it OK to use a thread pool thread for an event pump that runs indefinitely?
            Task.Run(() => ProcessDiagramShapeActionsAsync(_layoutEngineCancellation.Token));
        }

        public void Dispose()
        {
            _layoutEngineCancellation.Cancel();
            _layoutEngineCancellation.Dispose();
        }

        public void EnqueueDiagramAction(DiagramAction diagramAction)
        {
            lock (_diagramActionQueue)
            {
                _diagramActionQueue.Enqueue(diagramAction);
            }

            _diagramActionArrivedEvent.Set();
        }

        public void Clear()
        {
            _incrementalLayoutCalculator?.Clear();
        }

        private async void ProcessDiagramShapeActionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_diagramActionArrivedEvent.WaitOne(TimeSpan.FromSeconds(1)))
                {
                    while (await AreEventsArrivingInRapidSuccessionAsync())
                    {
                    }

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

            var layoutActions = _incrementalLayoutCalculator.CalculateLayoutActions(diagramActions).ToList();

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