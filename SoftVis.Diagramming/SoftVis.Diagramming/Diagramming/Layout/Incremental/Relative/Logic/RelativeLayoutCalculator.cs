using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute.Logic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// Calculates the arrangement of layout vertices relative to each other.
    /// </summary>
    /// <remarks>
    /// Arranges vertices into layers so that:
    /// <para>all edges point "upward" (to a layer with lower index)</para>
    /// <para>all edges span exactly 2 layers (by using dummy vertices as necessary)</para>
    /// <para>vertices in all layers ar ordered so that primary edges never cross.</para>
    /// </remarks>
    internal class RelativeLayoutCalculator : RelativeLayoutActionEventSource, IDiagramChangeConsumer
    {
        private readonly LayeredLayoutGraph _layoutGraph;
        private readonly LayoutVertexLayers _layers;
        private readonly RelativeLayout _relativeLayout;

        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly RelativeLocationCalculator _locationCalculator;
        private readonly AbsoluteLayoutCalculator _absoluteLayoutCalculator;

        public RelativeLayoutCalculator()
        {
            _layoutGraph = new LayeredLayoutGraph();
            _layers = new LayoutVertexLayers();
            _relativeLayout = new RelativeLayout(_layoutGraph, _layers);

            _diagramNodeToLayoutVertexMap = new Map<DiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<DiagramConnector, LayoutPath>();
            _locationCalculator = new RelativeLocationCalculator(
                _relativeLayout.ProperLayeredLayoutGraph, _relativeLayout.LayoutVertexLayers);

            _absoluteLayoutCalculator = new AbsoluteLayoutCalculator(RelativeLayout, DiagramDefaults.HorizontalGap, DiagramDefaults.VerticalGap);
            _absoluteLayoutCalculator.LayoutActionExecuted += RaiseLayoutAction;
        }

        public IReadOnlyQuasiProperLayoutGraph ProperLayoutGraph => _layoutGraph.ProperGraph;
        public IReadOnlyRelativeLayout RelativeLayout => _relativeLayout;

        public void OnDiagramCleared()
        {
            _layers.Clear();
            _layoutGraph.Clear();
            _diagramNodeToLayoutVertexMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
            _absoluteLayoutCalculator.OnLayoutCleared();
        }

        public void OnDiagramNodeAdded(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            if (_diagramNodeToLayoutVertexMap.ContainsKey(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            _layoutGraph.AddVertex(diagramNodeLayoutVertex);
            SetLocation(diagramNodeLayoutVertex, causingAction);

            _absoluteLayoutCalculator.OnVertexAdded(diagramNodeLayoutVertex,
                _layers.GetLocationOrThrow(diagramNodeLayoutVertex), causingAction);
        }

        public void OnDiagramNodeRemoved(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            if (!_diagramNodeToLayoutVertexMap.ContainsKey(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            _absoluteLayoutCalculator.OnVertexRemoved(diagramNodeLayoutVertex,
                _layers.GetLocationOrThrow(diagramNodeLayoutVertex), causingAction);

            _layers.RemoveVertex(diagramNodeLayoutVertex);
            _layoutGraph.RemoveVertex(diagramNodeLayoutVertex);
        }

        public void OnDiagramConnectorAdded(DiagramConnector diagramConnector, ILayoutAction causingAction)
        {
            Debug.WriteLine($"OnDiagramConnectorAdded {diagramConnector}");

            if (_diagramConnectorToLayoutPathMap.ContainsKey(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var layoutPath = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            var affectedVerticesBefore = GetAffectedVertices(layoutPath).ToList();

            var oldLocations = new Map<LayoutVertexBase, RelativeLocation>();
            affectedVerticesBefore.ForEach(i => SaveLocation(i, oldLocations));
            affectedVerticesBefore.ForEach(i => RemoveFromLayers(i, oldLocations));

            _layoutGraph.AddEdge(layoutPath);

            var affectedVerticesAfter = GetAffectedVertices(layoutPath)
                .OrderBy(ProperLayoutGraph.GetLayerIndex).ToList();
            affectedVerticesAfter.ForEach(i => SetLocation(i, causingAction, oldLocations));

            foreach (var vertex in affectedVerticesBefore.Union(affectedVerticesAfter))
            {
                var oldLocation = oldLocations.ContainsKey(vertex) 
                    ? oldLocations.Get(vertex) 
                    : (RelativeLocation?)null;

                var newLocation = _layers.GetLocation(vertex);

                if (oldLocation == null && newLocation != null)
                    _absoluteLayoutCalculator.OnVertexAdded(vertex, newLocation.Value, causingAction);
                else if (oldLocation != null && newLocation == null)
                    _absoluteLayoutCalculator.OnVertexRemoved(vertex, oldLocation.Value, causingAction);
                else if (oldLocation != null && oldLocation.Value != newLocation.Value)
                    _absoluteLayoutCalculator.OnVertexMoved(vertex, oldLocation.Value, newLocation.Value, causingAction);
            }

            _absoluteLayoutCalculator.OnPathAdded(layoutPath, causingAction);
        }

        private IEnumerable<LayoutVertexBase> GetAffectedVertices(LayoutPath layoutPath)
        {
            var diagramNodeVertices = _layoutGraph.GetVertexAndDescendants(layoutPath.PathSource).ToList();
            var dummyVertices = diagramNodeVertices.SelectMany(i => _layoutGraph.OutEdges(i))
                .SelectMany(i => i.InterimVertices);
            return diagramNodeVertices.Concat((IEnumerable<LayoutVertexBase>)dummyVertices);
        }

        private void SaveLocation(LayoutVertexBase vertex, Map<LayoutVertexBase, RelativeLocation> oldLocations)
        {
            var oldLocation = _layers.GetLocation(vertex);
            if (oldLocation != null)
                oldLocations.Set(vertex, oldLocation.Value);
        }

        private void RemoveFromLayers(LayoutVertexBase vertex, Map<LayoutVertexBase, RelativeLocation> oldLocations)
        {
            if (oldLocations.ContainsKey(vertex))
                _layers.RemoveVertex(vertex);
        }

        private void SetLocation(LayoutVertexBase vertex, ILayoutAction causingAction,
            Map<LayoutVertexBase, RelativeLocation> oldLocations = null)
        {
            if (!ProperLayoutGraph.ContainsVertex(vertex))
                return;

            var oldLocation = oldLocations != null && oldLocations.ContainsKey(vertex)
                ? oldLocations.Get(vertex)
                : (RelativeLocation?)null;

            var targetLocation = _locationCalculator.GetTargetLocation(vertex);
            _layers.AddVertex(vertex, targetLocation);

            if (oldLocation == null)
                RaiseRelativeLocationAssignedLayoutAction(vertex, targetLocation, causingAction);
            else
                RaiseRelativeLocationChangedLayoutAction(vertex, oldLocation.Value, targetLocation, causingAction);

            Debug.WriteLine($"SetRelativeLocation {vertex} to {targetLocation}");
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