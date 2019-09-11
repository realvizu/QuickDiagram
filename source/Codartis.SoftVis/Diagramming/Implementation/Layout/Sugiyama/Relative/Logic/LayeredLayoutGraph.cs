using System;
using Codartis.SoftVis.Graphs.Layered;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic
{
    /// <summary>
    /// A layered graph used for layout calculation.
    /// Also maintains a proper layered graph.
    /// </summary>
    /// <remarks>
    /// Invariants:
    /// <para>All operations leave the proper graph in proper state.</para>
    /// </remarks>
    internal class LayeredLayoutGraph : LayeredGraph<DiagramNodeLayoutVertex, LayoutPath>, 
        IReadOnlyLayeredLayoutGraph
    {
        private readonly QuasiProperLayoutGraph _properGraph;
        private int _nextDummyVertexId = 1;
        private readonly object _nextDummyVertexIdLockObject = new object();

        public LayeredLayoutGraph()
        {
            _properGraph = new QuasiProperLayoutGraph();
            Cleared += OnCleared;
        }

        public IReadOnlyQuasiProperLayoutGraph ProperGraph => _properGraph;

        private void OnCleared(object sender, EventArgs args)
        {
            _properGraph.Clear();
        }

        public override bool AddVertex(DiagramNodeLayoutVertex vertex)
        {
            if (!base.AddVertex(vertex))
                return false;

            _properGraph.AddVertex(vertex);

            CheckProperGraph();
            return true;
        }

        public override bool RemoveVertex(DiagramNodeLayoutVertex vertex)
        {
            if (!base.RemoveVertex(vertex))
                return false;

            _properGraph.RemoveVertex(vertex);

            CheckProperGraph();
            return true;
        }

        public override bool AddEdge(LayoutPath edge)
        {
            if (!base.AddEdge(edge))
                return false;

            AddEdgeToProperGraph(edge);

            CheckProperGraph();
            return true;
        }

        public override bool RemoveEdge(LayoutPath edge)
        {
            if (!base.RemoveEdge(edge))
                return false;

            RemoveEdgeFromProperGraph(edge);

            CheckProperGraph();
            return true;
        }

        private void AddEdgeToProperGraph(LayoutPath layoutPath)
        {
            foreach (var dummyLayoutVertex in layoutPath.InterimVertices)
                _properGraph.AddVertex(dummyLayoutVertex);

            foreach (var layoutEdge in layoutPath)
                _properGraph.AddEdge(layoutEdge);

            ExecuteOnVertexAndDescendants(layoutPath.PathSource, AdjustPaths);
        }

        private void RemoveEdgeFromProperGraph(LayoutPath layoutPath)
        {
            foreach (var layoutEdge in layoutPath)
                _properGraph.RemoveEdge(layoutEdge);

            foreach (var interimVertex in layoutPath.InterimVertices)
                _properGraph.RemoveVertex(interimVertex);

            ExecuteOnVertexAndDescendants(layoutPath.PathSource, AdjustPaths);
        }

        private void AdjustPaths(DiagramNodeLayoutVertex diagramNodeLayoutVertex)
        {
            foreach (var outEdge in OutEdges(diagramNodeLayoutVertex))
                AdjustPathLength(outEdge);
        }

        private void AdjustPathLength(LayoutPath layoutPath)
        {
            var sourceLayerIndex = GetLayerIndex(layoutPath.PathSource);
            var targetLayerIndex = GetLayerIndex(layoutPath.PathTarget);

            var layerSpan = sourceLayerIndex - targetLayerIndex;
            var pathLengthDifference = layerSpan - layoutPath.Length;

            if (pathLengthDifference > 0)
                SplitEdge(layoutPath, 0, pathLengthDifference);
            else if (pathLengthDifference < 0)
                MergeEdgeWithNext(layoutPath, 0, -pathLengthDifference);
        }

        private void SplitEdge(LayoutPath layoutPath, int atIndex, int times)
        {
            for (var i = 0; i < times; i++)
                SplitEdge(layoutPath, atIndex);
        }

        private void SplitEdge(LayoutPath layoutPath, int atIndex)
        {
            var edgeToSplit = layoutPath[atIndex];
            var interimVertex = CreateDummyVertex();

            var newEdges = SplitEdge(edgeToSplit, interimVertex);
            layoutPath.Substitute(atIndex, 1, newEdges[0], newEdges[1]);
        }

        private GeneralLayoutEdge[] SplitEdge(GeneralLayoutEdge edgeToSplit, LayoutVertexBase interimVertex)
        {
            var newEdge1 = new GeneralLayoutEdge(edgeToSplit.Source, interimVertex, edgeToSplit.DiagramConnector);
            var newEdge2 = new GeneralLayoutEdge(interimVertex, edgeToSplit.Target, edgeToSplit.DiagramConnector);

            _properGraph.RemoveEdge(edgeToSplit);
            _properGraph.AddVertex(interimVertex);
            _properGraph.AddEdge(newEdge1);
            _properGraph.AddEdge(newEdge2);

            return new[] { newEdge1, newEdge2 };
        }

        private void MergeEdgeWithNext(LayoutPath layoutPath, int atIndex, int times)
        {
            for (var i = 0; i < times; i++)
                MergeEdgeWithNext(layoutPath, atIndex);
        }

        private void MergeEdgeWithNext(LayoutPath layoutPath, int atIndex)
        {
            var firstEdge = layoutPath[atIndex];
            var nextEdge = layoutPath[atIndex + 1];

            var vertexToRemove = firstEdge.Target as DummyLayoutVertex;
            if (vertexToRemove == null)
                throw new Exception("FirstEdge.Target is null or not dummy!");

            var mergedEdge = MergeEdges(firstEdge, nextEdge);
            layoutPath.Substitute(atIndex, 2, mergedEdge);
        }

        private GeneralLayoutEdge MergeEdges(GeneralLayoutEdge firstEdge, GeneralLayoutEdge nextEdge)
        {
            if (firstEdge.Target != nextEdge.Source)
                throw new InvalidOperationException("Only consecutive edges can be merged.");

            if (firstEdge.DiagramConnector != nextEdge.DiagramConnector)
                throw new InvalidOperationException("Only edges of the same DiagramConnector can be merged.");

            var mergedEdge = new GeneralLayoutEdge(firstEdge.Source, nextEdge.Target, firstEdge.DiagramConnector);

            _properGraph.RemoveEdge(firstEdge);
            _properGraph.RemoveEdge(nextEdge);
            _properGraph.RemoveVertex(firstEdge.Target);
            _properGraph.AddEdge(mergedEdge);

            return mergedEdge;
        }

        private void CheckProperGraph()
        {
            if (!_properGraph.IsProper())
                throw new Exception("The proper graph is not proper.");
        }

        private DummyLayoutVertex CreateDummyVertex() => new DummyLayoutVertex(GetNextDummyVertexId());

        private int GetNextDummyVertexId()
        {
            lock (_nextDummyVertexIdLockObject)
            {
                var id = _nextDummyVertexId;
                _nextDummyVertexId++;
                return id;
            }
        }
    }
}
