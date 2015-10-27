using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A graph created for vertex position calculation.
    /// It has two types of vertices: some represent diagram nodes, others are dummies introduced to 
    /// break long edges so all edges span only 2 layers.
    /// </summary>
    internal sealed class PositioningGraph : BidirectionalGraph<PositioningVertexBase, PositioningEdge>
    {
        public PositioningGraph() 
            : base(allowParallelEdges: false)
        {
        }

        public void AddPath(PositioningEdgePath path)
        {
            foreach (var interimVertex in path.InterimVertices)
                AddVertex(interimVertex);

            foreach (var edge in path)
                AddEdge(edge);

            path.EdgeAdded += OnPathEdgeAdded;
            path.EdgeRemoved += OnPathEdgeRemoved;
            path.InterimVertexAdded += OnPathInterimVertexAdded;
            path.InterimVertexRemoved += OnPathInterimVertexRemoved;
        }

        public void RemovePath(PositioningEdgePath path)
        {
            path.EdgeAdded -= OnPathEdgeAdded;
            path.EdgeRemoved -= OnPathEdgeRemoved;
            path.InterimVertexAdded -= OnPathInterimVertexAdded;
            path.InterimVertexRemoved -= OnPathInterimVertexRemoved;

            foreach (var edge in path)
                RemoveEdge(edge);

            foreach (var interimVertex in path.InterimVertices)
                RemoveVertex(interimVertex);
        }

        private void OnPathEdgeAdded(object sender, PositioningEdge edge)
        {
            AddEdge(edge);
        }

        private void OnPathEdgeRemoved(object sender, PositioningEdge edge)
        {
            RemoveEdge(edge);
        }

        private void OnPathInterimVertexAdded(object sender, DummyPositioningVertex vertex)
        {
            AddVertex(vertex);
        }

        private void OnPathInterimVertexRemoved(object sender, DummyPositioningVertex vertex)
        {
            RemoveVertex(vertex);
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
            var layerIndex = vertex.LayerIndex;

            return GetParents(vertex)
                .OrderByDescending(i => i.Priority)
                .ThenBy(i => layerIndex - i.LayerIndex)
                .ThenBy(i => i.ToString());
        }
    }
}
