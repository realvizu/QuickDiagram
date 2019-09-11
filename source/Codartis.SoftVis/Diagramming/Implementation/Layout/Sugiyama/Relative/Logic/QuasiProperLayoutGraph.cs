using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Graphs.Layered;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic
{
    /// <summary>
    /// A layered graph that can determine whether it's proper or not. 
    /// Contains normal vertices for diagram nodes and dummy vertices for connector routing.
    /// </summary>
    /// <remarks>
    /// Terms:
    /// <para>Proper layering: the source and target vertex of each edge is exactly 1 layer away.</para>
    /// <para>Primary parent: the first in the ordering of the parent vertices.</para>
    /// <para>Primary children: those children whose primary parent is this vertex.</para>
    /// <para>Primary siblings: those vertices that have the same primary parent.</para>
    /// Invariants:
    /// <para>Dummy vertices' in/out degree (number of in/out edges) must be at most 1.</para>
    /// </remarks>
    internal class QuasiProperLayoutGraph : LayeredGraph<LayoutVertexBase, GeneralLayoutEdge>,
        IReadOnlyQuasiProperLayoutGraph
    {
        public bool IsProper() => Vertices.All(AreAllRelationshipsProper);

        public override bool AddVertex(LayoutVertexBase vertex)
        {
            if (!base.AddVertex(vertex))
                return false;

            CheckInvariants();
            return true;
        }

        public override bool RemoveVertex(LayoutVertexBase vertex)
        {
            if (!base.RemoveVertex(vertex))
                return false;

            CheckInvariants();
            return true;
        }

        public override bool AddEdge(GeneralLayoutEdge edge)
        {
            if (!base.AddEdge(edge))
                return false;

            CheckInvariants();
            return true;
        }

        public override bool RemoveEdge(GeneralLayoutEdge edge)
        {
            if (!base.RemoveEdge(edge))
                return false;

            CheckInvariants();
            return true;
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

        public IEnumerable<LayoutVertexBase> GetPrimarySiblings(LayoutVertexBase vertex)
        {
            var primaryParent = GetPrimaryParent(vertex);
            if (primaryParent == null)
                return Enumerable.Empty<LayoutVertexBase>();

            return GetPrimaryChildren(primaryParent).Where(i => i != vertex);
        }

        public GeneralLayoutEdge GetInEdge(DummyLayoutVertex dummyVertex) => InEdges(dummyVertex).FirstOrDefault();
        public GeneralLayoutEdge GetOutEdge(DummyLayoutVertex dummyVertex) => OutEdges(dummyVertex).FirstOrDefault();

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

        private bool AreAllRelationshipsProper(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);

            return GetParents(vertex).All(i => GetLayerIndex(i) == layerIndex - 1)
               && GetChildren(vertex).All(i => GetLayerIndex(i) == layerIndex + 1)
               && GetSiblings(vertex).All(i => GetLayerIndex(i) == layerIndex);
        }

        private void CheckInvariants()
        {
            Edges.ForEach(CheckEdgeInvariants);
        }

        private void CheckEdgeInvariants(GeneralLayoutEdge edge)
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
