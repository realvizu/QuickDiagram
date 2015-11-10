using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// A detailed graph for layout calculation. 
    /// Contains normal vertices for diagram nodes and dummy vertices for connector routing.
    /// </summary>
    /// <remarks>
    /// Responsibilities:
    /// <para>Understands primary parent/children/sibling relationships.</para>
    /// <para>Understands that vertices can be placed or floating.</para>
    /// Invariants:
    /// <para>Dummy vertices' in/out degree (number of in/out edges) must be at most 1.</para>
    /// </remarks>
    internal class LowLevelLayoutGraph : LayoutGraphBase<LayoutVertexBase, LayoutEdge>,
        IReadOnlyLowLevelLayoutGraph
    {
        public override bool AddEdge(LayoutEdge edge)
        {
            var isAdded = base.AddEdge(edge);
            if (isAdded)
                CheckDegreeLimits(edge);
            
            return isAdded;
        }

        public void AddPath(LayoutPath layoutPath)
        {
            foreach (var interimVertex in layoutPath.InterimVertices)
                AddVertex(interimVertex);

            foreach (var edge in layoutPath)
                AddEdge(edge);
        }

        public void RemovePath(LayoutPath layoutPath)
        {
            foreach (var edge in layoutPath)
                RemoveEdge(edge);

            foreach (var interimVertex in layoutPath.InterimVertices)
                RemoveVertex(interimVertex);
        }

        public LayoutVertexBase GetPrimaryParent(LayoutVertexBase vertex)
        {
            return GetOrderedParents(vertex).FirstOrDefault();
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

        public void FloatTree(LayoutVertexBase vertex)
        {
            ExecuteOnDescendantVertices(vertex, i=>i.IsFloating = true);
        }

        public void FloatPrimaryTree(LayoutVertexBase vertex)
        {
            ExecuteOnPrimaryDescendantVertices(vertex, i => i.IsFloating = true);
        }

        public void ExecuteOnPrimaryDescendantVertices(LayoutVertexBase rootVertex, Action<LayoutVertexBase> actionOnVertex)
        {
            actionOnVertex(rootVertex);

            foreach (var child in GetPrimaryChildren(rootVertex))
                ExecuteOnPrimaryDescendantVertices(child, actionOnVertex);
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
