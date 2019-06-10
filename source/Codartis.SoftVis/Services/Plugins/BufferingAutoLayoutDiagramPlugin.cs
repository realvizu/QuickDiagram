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

                    Debug.WriteLine($"Calling layout.");
                    DoLayout();
                    Debug.WriteLine($"Calling layout done.");
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
            lock (_diagramEventQueue)
                return _diagramEventQueue.Count;
        }

        private void DoLayout()
        {
            var diagram = DiagramService.Diagram;
            var nodes = diagram.Nodes.Where(i => !i.HasParent).ToArray();
            var connectors = diagram.Connectors.Where(i => nodes.Contains(i.Source) && nodes.Contains(i.Target));

            var oldPositions = GetCenterPositions(nodes);
            var newPositions = _layoutAlgorithm.Calculate(nodes, connectors);

            var changedPositions = GetPositionChanges(oldPositions, newPositions);
            Debug.WriteLine($"ChangedPositions.Count={changedPositions.Count}");

            ApplyChanges(changedPositions, diagram);
        }

        private static IDictionary<ModelNodeId, Point2D> GetTopLeftPositions(IDiagramNode[] nodes)
        {
            return nodes.ToDictionary(i => i.Id, i => i.TopLeft);
        }

        private static IDictionary<ModelNodeId, Point2D> GetCenterPositions(IDiagramNode[] nodes)
        {
            return nodes.ToDictionary(i => i.Id, i => i.Center);
        }

        private static IDictionary<ModelNodeId, Point2D> GetPositionChanges(IDictionary<ModelNodeId, Point2D> oldPositions, IDictionary<ModelNodeId, Point2D> newPositions)
        {
            var query =
                from oldPosition in oldPositions
                join newPosition in newPositions on oldPosition.Key equals newPosition.Key
                where oldPosition.Value != newPosition.Value && newPosition.Value != Point2D.Undefined
                select newPosition;

            return query.ToDictionary(i => i.Key, i => i.Value);
        }

        private void ApplyChanges(IDictionary<ModelNodeId, Point2D> changedPositions, IDiagram diagram)
        {
            foreach (var changedPosition in changedPositions)
            {
                var diagramNode = diagram.GetNode(changedPosition.Key);
                DiagramService.UpdateDiagramNodeCenter(diagramNode, changedPosition.Value);
            }
        }
    }
}