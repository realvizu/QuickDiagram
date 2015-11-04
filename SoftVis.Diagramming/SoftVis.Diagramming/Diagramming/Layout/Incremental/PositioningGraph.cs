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
        public PositioningGraph() 
            : base(allowParallelEdges: false)
        {
        }

        public override bool AddEdge(PositioningEdge edge)
        {
            var isAdded = base.AddEdge(edge);
            if (isAdded)
                CheckDegreeLimits(edge);
            
            return isAdded;
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

        public bool IsPlacedPrimarySiblingOf(PositioningVertexBase vertex1, PositioningVertexBase vertex2)
        {
            return vertex2 != null 
                && !vertex2.IsFloating 
                && GetPrimaryParent(vertex1) == GetPrimaryParent(vertex2);
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

        public PositioningEdge GetInEdge(DummyPositioningVertex dummyVertex) => InEdges(dummyVertex).FirstOrDefault();
        public PositioningEdge GetOutEdge(DummyPositioningVertex dummyVertex) => OutEdges(dummyVertex).FirstOrDefault();

        private IEnumerable<PositioningVertexBase> GetOrderedParents(PositioningVertexBase vertex)
        {
            return GetParents(vertex)
                .OrderByDescending(GetVertexPriority)
                .ThenBy(GetDistanceToNonDummy)
                .ThenBy(i => i.ToString());
        }

        private int GetVertexPriority(PositioningVertexBase vertex)
        {
            var dummyPositioningVertex = vertex as DummyPositioningVertex;
            if (dummyPositioningVertex == null)
                return vertex.Priority;

            var outEdge = GetOutEdge(dummyPositioningVertex);
            if (outEdge == null)
                throw new InvalidOperationException("Dummy vertex with no out-edge does not have priority.");
            return GetVertexPriority(outEdge.Target);
        }

        private int GetDistanceToNonDummy(PositioningVertexBase vertex)
        {
            var dummyPositioningVertex = vertex as DummyPositioningVertex;
            if (dummyPositioningVertex == null)
                return 1;

            var outEdge = GetOutEdge(dummyPositioningVertex);
            if (outEdge == null)
                throw new InvalidOperationException("Dummy vertex with no out-edge does not have distance to non-dummy.");
            return GetDistanceToNonDummy(outEdge.Target) + 1;
        }

        private void CheckDegreeLimits(PositioningEdge edge)
        {
            CheckDegree(edge.Source, EdgeDirection.In);
            CheckDegree(edge.Source, EdgeDirection.Out);
            CheckDegree(edge.Target, EdgeDirection.In);
            CheckDegree(edge.Target, EdgeDirection.Out);
        }

        private void CheckDegree(PositioningVertexBase vertex, EdgeDirection direction)
        {
            if (vertex.IsDummy && this.Degree(vertex, direction) > 1)
                throw new InvalidOperationException($"Dummy vertex {vertex} cannot have more than 1 {direction}-edges.");
        }
    }
}
