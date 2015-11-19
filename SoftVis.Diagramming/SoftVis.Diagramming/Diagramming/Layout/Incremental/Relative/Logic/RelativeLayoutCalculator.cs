using System;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// Calculates the arrangement of layout vertices relative to each other.
    /// </summary>
    /// <remarks>
    /// Arranges vertices into layers so that:
    /// <para>all edges point "upward" (to a layer with lower index),</para>
    /// <para>all edges span exactly 2 layers (by using dummy vertices as necessary)</para>
    /// <para>vertices in all layers ar ordered so that primary edges never cross.</para>
    /// No primary edge crossing is achieved by:
    /// <para>dummy vertices are inserted to ensure that parent and children are always on adjacent layers,</para>
    /// <para>primary siblings on a layer are always placed together to form a block,</para>
    /// <para>if a vertex has no primary siblings on its layer but has a primary parent 
    /// then it is ordered according to parent ordering.</para>
    /// </remarks>
    internal class RelativeLayoutCalculator : RelativeLayoutActionEventSource, IDiagramChangeConsumer
    {
        private readonly LayeredLayoutGraph _layoutGraph;
        private readonly LayoutVertexLayers _layers;
        private readonly RelativeLayout _relativeLayout;

        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly RelativeLocationCalculator _locationCalculator;

        public RelativeLayoutCalculator()
        {
            _layoutGraph = new LayeredLayoutGraph();
            _layers = new LayoutVertexLayers();
            _relativeLayout = new RelativeLayout(_layoutGraph, _layers);

            _diagramNodeToLayoutVertexMap = new Map<DiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<DiagramConnector, LayoutPath>();
            _locationCalculator = new RelativeLocationCalculator(
                _relativeLayout.ProperLayeredLayoutGraph, _relativeLayout.LayoutVertexLayers);
        }

        public IReadOnlyQuasiProperLayoutGraph ProperLayoutGraph => _layoutGraph.ProperGraph;
        public IReadOnlyRelativeLayout RelativeLayout => _relativeLayout;

        public void OnDiagramCleared()
        {
            _layers.Clear();
            _layoutGraph.Clear();
            _diagramNodeToLayoutVertexMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
        }

        public void OnDiagramNodeAdded(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            if (_diagramNodeToLayoutVertexMap.ContainsKey(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            AddVertex(diagramNodeLayoutVertex, causingAction);
        }

        public void OnDiagramNodeRemoved(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            if (!_diagramNodeToLayoutVertexMap.ContainsKey(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            RemoveVertex(diagramNodeLayoutVertex);
        }

        public void OnDiagramConnectorAdded(DiagramConnector diagramConnector, ILayoutAction causingAction)
        {
            if (_diagramConnectorToLayoutPathMap.ContainsKey(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var diagramNodeLayoutEdge = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, diagramNodeLayoutEdge);

            _layoutGraph.AddEdge(diagramNodeLayoutEdge);
        }

        public void OnDiagramConnectorRemoved(DiagramConnector diagramConnector, ILayoutAction causingAction)
        {
            if (!_diagramConnectorToLayoutPathMap.ContainsKey(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} not found.");

            var diagramNodeLayoutEdge = _diagramConnectorToLayoutPathMap.Get(diagramConnector);
            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);

            _layoutGraph.RemoveEdge(diagramNodeLayoutEdge);
        }

        private LayoutPath CreateLayoutPath(DiagramConnector diagramConnector)
        {
            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            return new LayoutPath(sourceVertex, targetVertex, diagramConnector);
        }

        private void AddVertex(DiagramNodeLayoutVertex vertex, ILayoutAction causingAction)
        {
            _layoutGraph.AddVertex(vertex);

            var targetLocation = _locationCalculator.GetTargetLocation(vertex);
            _layers.AddVertex(vertex, targetLocation);

            RaiseRelativeLocationAssignedLayoutAction(vertex, targetLocation, causingAction);
        }

        private void RemoveVertex(DiagramNodeLayoutVertex vertex)
        {
            _layers.RemoveVertex(vertex);
            _layoutGraph.RemoveVertex(vertex);
        }

        private void EnsureCorrectLocation(LayoutVertexBase vertex, ILayoutAction causingAction)
        {
            var currentLocation = _layers.GetLocation(vertex);
            var targetLocation = _locationCalculator.GetTargetLocation(vertex);

            if (currentLocation != targetLocation)
                MoveVertex(vertex, targetLocation, causingAction);
        }

        private void MoveVertex(LayoutVertexBase vertex, RelativeLocation targetLocation, ILayoutAction causingAction)
        {
            var oldLocation = _layers.GetLocationOrThrow(vertex);

            _layers.RemoveVertex(vertex);
            _layers.AddVertex(vertex, targetLocation);

            RaiseRelativeLocationChangedLayoutAction(vertex, oldLocation, targetLocation, causingAction);
        }
    }
}