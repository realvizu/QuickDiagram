using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A graph created for layout calculation. 
    /// </summary>
    /// <remarks>
    /// Understands parent/child/sibling and primary parent/children/sibling relationships.
    /// Understands that vertices can be placed or floating.
    /// There are two types of vertices: normals represent diagram nodes, dummies are introduced to break long edges.
    /// Dummy vertices cannot have more than 1 in and out edges.
    /// </remarks>
    internal sealed class LayoutGraph : BidirectionalGraph<LayoutVertexBase, LayoutEdge>,
        IReadOnlyLayoutGraph
    {
        public LayoutGraph() 
            : base(allowParallelEdges: false)
        {
        }

        public override bool AddEdge(LayoutEdge edge)
        {
            var isAdded = base.AddEdge(edge);
            if (isAdded)
                CheckDegreeLimits(edge);
            
            return isAdded;
        }

        public void AddPath(LayoutPath path)
        {
            foreach (var interimVertex in path.InterimVertices)
                AddVertex(interimVertex);

            foreach (var edge in path)
                AddEdge(edge);
        }

        public void RemovePath(LayoutPath path)
        {
            foreach (var edge in path)
                RemoveEdge(edge);

            foreach (var interimVertex in path.InterimVertices)
                RemoveVertex(interimVertex);
        }

        public IEnumerable<LayoutVertexBase> GetParents(LayoutVertexBase vertex)
        {
            return this.GetOutNeighbours(vertex);
        }

        public LayoutVertexBase GetPrimaryParent(LayoutVertexBase vertex)
        {
            return GetOrderedParents(vertex).FirstOrDefault();
        }

        public IEnumerable<LayoutVertexBase> GetChildren(LayoutVertexBase vertex)
        {
            return this.GetInNeighbours(vertex);
        }

        public bool HasPrimaryChildren(LayoutVertexBase vertex)
        {
            return GetPrimaryChildren(vertex).Any();
        }

        public IEnumerable<LayoutVertexBase> GetPrimaryChildren(LayoutVertexBase vertex)
        {
            return GetChildren(vertex).Where(i => GetPrimaryParent(i) == vertex);
        }

        public IEnumerable<LayoutVertexBase> GetPlacedPrimaryChildren(LayoutVertexBase vertex)
        {
            return GetPrimaryChildren(vertex).Where(i => !i.IsFloating);
        }

        public IEnumerable<LayoutVertexBase> GetPrimarySiblings(LayoutVertexBase vertex)
        {
            var primaryParent = GetPrimaryParent(vertex);
            if (primaryParent == null)
                return Enumerable.Empty<LayoutVertexBase>();

            return GetPrimaryChildren(primaryParent).Where(i => i != vertex);
        }

        public bool IsPlacedPrimarySiblingOf(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return vertex2 != null 
                && !vertex2.IsFloating 
                && GetPrimaryParent(vertex1) == GetPrimaryParent(vertex2);
        }

        public IEnumerable<LayoutEdge> GetPrimaryEdges(LayoutVertexBase vertex)
        {
            return InEdges(vertex).Where(i => GetPrimaryParent(i.Source) == vertex);
        }

        public void ExecuteOnPrimaryDescendantVertices(LayoutVertexBase rootVertex, Action<LayoutVertexBase> actionOnVertex)
        {
            actionOnVertex(rootVertex);
            foreach (var layoutEdge in GetPrimaryEdges(rootVertex))
            {
                var nextVertex = layoutEdge.Source;
                ExecuteOnPrimaryDescendantVertices(nextVertex, actionOnVertex);
            }
        }

        public LayoutEdge GetInEdge(DummyLayoutVertex dummyVertex) => InEdges(dummyVertex).FirstOrDefault();
        public LayoutEdge GetOutEdge(DummyLayoutVertex dummyVertex) => OutEdges(dummyVertex).FirstOrDefault();

        private IEnumerable<LayoutVertexBase> GetOrderedParents(LayoutVertexBase vertex)
        {
            return GetParents(vertex)
                .OrderByDescending(GetVertexPriority)
                .ThenBy(GetDistanceToNonDummy)
                .ThenBy(i => i.ToString());
        }

        private int GetVertexPriority(LayoutVertexBase vertex)
        {
            var dummyVertex = vertex as DummyLayoutVertex;
            if (dummyVertex == null)
                return vertex.Priority;

            var outEdge = GetOutEdge(dummyVertex);
            if (outEdge == null)
                throw new InvalidOperationException("Dummy vertex with no out-edge does not have priority.");
            return GetVertexPriority(outEdge.Target);
        }

        private int GetDistanceToNonDummy(LayoutVertexBase vertex)
        {
            var dummyVertex = vertex as DummyLayoutVertex;
            if (dummyVertex == null)
                return 1;

            var outEdge = GetOutEdge(dummyVertex);
            if (outEdge == null)
                throw new InvalidOperationException("Dummy vertex with no out-edge does not have distance to non-dummy.");
            return GetDistanceToNonDummy(outEdge.Target) + 1;
        }

        private void CheckDegreeLimits(LayoutEdge edge)
        {
            CheckDegree(edge.Source, EdgeDirection.In);
            CheckDegree(edge.Source, EdgeDirection.Out);
            CheckDegree(edge.Target, EdgeDirection.In);
            CheckDegree(edge.Target, EdgeDirection.Out);
        }

        private void CheckDegree(LayoutVertexBase vertex, EdgeDirection direction)
        {
            if (vertex.IsDummy && this.Degree(vertex, direction) > 1)
                throw new InvalidOperationException($"Dummy vertex {vertex} cannot have more than 1 {direction}-edges.");
        }
    }
}
