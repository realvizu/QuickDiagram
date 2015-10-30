using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A graph created for vertex position calculation.
    /// </summary>
    /// <remarks>
    /// It has two types of vertices: some represent diagram nodes, others are dummies introduced to 
    /// break long edges so all edges span exactly 2 layers.
    /// </remarks>
    internal sealed class PositioningGraph : BidirectionalGraph<PositioningVertexBase, PositioningEdge>,
        IReadOnlyPositioningGraph
    {
        private readonly PositioningVertexLayers _vertexLayers;

        public PositioningGraph(double horizontalGap, double verticalGap) 
            : base(allowParallelEdges: false)
        {
            _vertexLayers = new PositioningVertexLayers(verticalGap);

            Cleared += OnCleared;
        }

        public IEnumerable<IReadOnlyPositioningVertexLayer> Layers => _vertexLayers;

        private void OnCleared(object sender, EventArgs args)
        {
            _vertexLayers.Clear();
        }

        public override bool AddVertex(PositioningVertexBase vertex)
        {
            var isAdded = base.AddVertex(vertex);
            if (isAdded)
                _vertexLayers.AddVertex(vertex);
            
            return isAdded;
        }

        public override bool RemoveVertex(PositioningVertexBase vertex)
        {
            var isRemoved = base.RemoveVertex(vertex);
            if (isRemoved)
                _vertexLayers.RemoveVertex(vertex);
            
            return isRemoved;
        }

        public override bool AddEdge(PositioningEdge edge)
        {
            var isAdded = base.AddEdge(edge);
            if (isAdded)
                edge.ExecuteOnDescendantEdges(i => _vertexLayers.EnsureValidLayering(i.Source, i.Target));
            
            return isAdded;
        }

        public override bool RemoveEdge(PositioningEdge edge)
        {
            return base.RemoveEdge(edge);
        }

        public void AddPath(PositioningEdgePath path)
        {
            foreach (var interimVertex in path.InterimVertices)
                AddVertex(interimVertex);

            foreach (var edge in path)
                AddEdge(edge);
        }

        public void RemovePath(PositioningEdgePath path)
        {
            foreach (var edge in path)
                RemoveEdge(edge);

            foreach (var interimVertex in path.InterimVertices)
                RemoveVertex(interimVertex);
        }

        public IEnumerable<PositioningVertexBase> GetParents(PositioningVertexBase vertex)
        {
            return this.GetOutNeighbours(vertex);
        }

        public PositioningVertexBase GetPrimaryParent(PositioningVertexBase vertex)
        {
            return GetOrderedParents(vertex).FirstOrDefault();
        }

        public IEnumerable<PositioningVertexBase> GetNonPrimaryParents(PositioningVertexBase vertex)
        {
            return GetOrderedParents(vertex).Skip(1);
        }

        public IEnumerable<PositioningVertexBase> GetChildren(PositioningVertexBase vertex)
        {
            return this.GetInNeighbours(vertex);
        }

        public IEnumerable<PositioningVertexBase> GetPrimaryChildren(PositioningVertexBase vertex)
        {
            return GetChildren(vertex).Where(i => GetPrimaryParent(i) == vertex);
        }

        public IEnumerable<PositioningVertexBase> GetPrimaryPositionedChildren(PositioningVertexBase vertex)
        {
            return GetPrimaryChildren(vertex).Where(i => !i.IsFloating);
        }

        public IEnumerable<PositioningVertexBase> GetPrimarySiblings(PositioningVertexBase vertex)
        {
            var primaryParent = GetPrimaryParent(vertex);
            if (primaryParent == null)
                return Enumerable.Empty<PositioningVertexBase>();

            return GetPrimaryChildren(primaryParent).Where(i => i != vertex);
        }

        public IEnumerable<PositioningEdge> GetPrimaryEdges(PositioningVertexBase vertex)
        {
            return InEdges(vertex).Where(i => GetPrimaryParent(i.Source) == vertex);
        }

        public void ExecuteOnPrimaryDescendantVertices(PositioningVertexBase rootVertex, Action<PositioningVertexBase> actionOnVertex)
        {
            actionOnVertex(rootVertex);
            foreach (var layoutEdge in GetPrimaryEdges(rootVertex))
            {
                var nextVertex = layoutEdge.Source;
                ExecuteOnPrimaryDescendantVertices(nextVertex, actionOnVertex);
            }
        }

        public void ExecuteOnDescendantEdges(PositioningEdge edge, Action<PositioningEdge> action)
        {
            this.ExecuteOnEdgesRecursive(edge, EdgeDirection.In, action);
        }

        private IEnumerable<PositioningVertexBase> GetOrderedParents(PositioningVertexBase vertex)
        {
            var nonDummyLayerIndex = vertex.NonDummyLayerIndex;

            return GetParents(vertex)
                .OrderByDescending(i => i.Priority)
                .ThenBy(i => nonDummyLayerIndex - i.NonDummyLayerIndex)
                .ThenBy(i => i.ToString());
        }

        public PositioningVertexLayer GetLayer(PositioningVertexBase vertex) => _vertexLayers.GetLayer(vertex);
        public int GetLayerIndex(PositioningVertexBase vertex) => _vertexLayers.GetLayerIndex(vertex);
    }
}
