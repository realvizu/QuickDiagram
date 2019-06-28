using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout.Nodes;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// A diagram that maintains it own layout.
    /// Responds to shape addition/removal and uses a layout engine that calculates how to arrange nodes and connectors.
    /// </summary>
    public sealed class BufferingAutoLayoutDiagramPlugin : DiagramPluginBase
    {
        private static readonly TimeSpan BufferingTimeSpan = TimeSpan.FromSeconds(1);

        private readonly INodeLayoutAlgorithm _layoutAlgorithm;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly Queue<DiagramEventBase> _diagramEventQueue;
        private readonly AutoResetEvent _diagramEventArrivedEvent;

        public BufferingAutoLayoutDiagramPlugin(INodeLayoutAlgorithm layoutAlgorithm)
        {
            _layoutAlgorithm = layoutAlgorithm;
            _cancellationToken = new CancellationTokenSource();
            _diagramEventQueue = new Queue<DiagramEventBase>();
            _diagramEventArrivedEvent = new AutoResetEvent(false);

            // BUGBUG: is it OK to use a thread pool thread for an event pump that runs indefinitely?
            Task.Run(() => ProcessDiagramShapeActionsAsync(_cancellationToken.Token));
        }

        public override void Initialize(IModelService modelService, IDiagramService diagramService)
        {
            base.Initialize(modelService, diagramService);

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            DiagramService.DiagramChanged -= OnDiagramChanged;

            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
        }

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            lock (_diagramEventQueue)
            {
                Debug.WriteLine($"Event arrived: {diagramEvent}");
                _diagramEventQueue.Enqueue(diagramEvent);
            }

            _diagramEventArrivedEvent.Set();
        }

        private async void ProcessDiagramShapeActionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_diagramEventArrivedEvent.WaitOne(BufferingTimeSpan))
                {
                    while (await AreEventsArrivingInRapidSuccessionAsync())
                    {
                    }

                    var lastEvent = GetLastEventFromQueue();
                    if (lastEvent == null)
                        return;

                    Debug.WriteLine($"Calling layout.");
                    DoLayout(lastEvent.NewDiagram);
                    Debug.WriteLine($"Calling layout done.");
                }
            }
        }

        private DiagramEventBase GetLastEventFromQueue()
        {
            DiagramEventBase lastEvent = null;

            lock (_diagramEventQueue)
            {
                while (_diagramEventQueue.Any())
                    lastEvent = _diagramEventQueue.Dequeue();
            }

            return lastEvent;
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
            lock (_diagramEventQueue)
                return _diagramEventQueue.Count;
        }

        private void DoLayout(IDiagram diagram)
        {
            var nodes = diagram.Nodes.Where(i => !i.HasParent).ToArray();
            var connectors = diagram.Connectors.Where(i => nodes.Contains(i.Source) && nodes.Contains(i.Target));

            var oldRects = GetRects(nodes);
            var newRects = _layoutAlgorithm.Calculate(nodes, connectors);
            var changedRects = GetChanges(oldRects, newRects);

            Debug.WriteLine($"ChangedRects.Count={changedRects.Count}");

            ApplyChanges(changedRects, diagram);
        }

        private static IDictionary<ModelNodeId, Rect2D> GetRects(IDiagramNode[] nodes)
        {
            return nodes.ToDictionary(i => i.Id, i => i.Rect);
        }

        private static IDictionary<ModelNodeId, Rect2D> GetChanges(IDictionary<ModelNodeId, Rect2D> oldRects, IDictionary<ModelNodeId, Rect2D> newRects)
        {
            var query =
                from oldRect in oldRects
                join newRect in newRects on oldRect.Key equals newRect.Key
                where oldRect.Value != newRect.Value && newRect.Value.IsDefined()
                select newRect;

            return query.ToDictionary(i => i.Key, i => i.Value);
        }

        private void ApplyChanges(IDictionary<ModelNodeId, Rect2D> changedRects, IDiagram diagram)
        {
            foreach (var rect in changedRects)
            {
                var diagramNode = diagram.GetNode(rect.Key);
                DiagramService.UpdateDiagramNodeCenter(diagramNode, rect.Value.Center);
            }
        }
    }
}