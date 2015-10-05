using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using QuickGraph;
using QuickGraph.Algorithms.Search;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.SimplifiedSugiyama
{
    internal sealed class LayoutGraph : BidirectionalGraph<LayoutVertex, LayoutEdge>
    {
        private readonly Dictionary<ISized, LayoutVertex> _originalToLayoutVertexMap;
        private readonly IDictionary<IEdge<ISized>, IList<LayoutVertex>> _edgeToDummyVerticesMap;

        public LayoutGraph(IEnumerable<ISized> originalVertice, IEnumerable<IEdge<ISized>> originalEdges)
        {
            _originalToLayoutVertexMap = originalVertice.ToDictionary(i => i, LayoutVertex.Create);
            _edgeToDummyVerticesMap = new Dictionary<IEdge<ISized>, IList<LayoutVertex>>();

            AddVertexRange(_originalToLayoutVertexMap.Values);
            AddEdgeRange(originalEdges.Select(CreateLayoutEdge));

            Normalize();
        }

        public IDictionary<IEdge<ISized>, Route> GetInterimRoutePointsOfEdges()
        {
            return _edgeToDummyVerticesMap.ToDictionary(i => i.Key, i => new Route(i.Value.Select(v => v.Center)));
        }

        public List<LayoutVertex> RemoveIsolatedVertices()
        {
            var isolatedVertices = Vertices.Where(i => Degree(i) == 0).ToList();
            isolatedVertices.ForEach(i => RemoveVertex(i));
            return isolatedVertices;
        }

        public void BreakEdgeWithInterimVertices(LayoutEdge edgeToBreak, List<LayoutVertex> interimVertices)
        {
            var interimVerticesToSave = edgeToBreak.IsReversed
                ? ((IEnumerable<LayoutVertex>)interimVertices).Reverse()
                : interimVertices;

            _edgeToDummyVerticesMap.Add(edgeToBreak.OriginalEdge, interimVerticesToSave.ToArray());

            RemoveEdge(edgeToBreak);

            AddVertexRange(interimVertices);

            interimVertices.Insert(0, edgeToBreak.Source);
            interimVertices.Add(edgeToBreak.Target);

            for (var i = 0; i < interimVertices.Count - 1; i++)
            {
                var newEdge = new LayoutEdge(edgeToBreak.OriginalEdge, interimVertices[i], interimVertices[i + 1], 
                    edgeToBreak.IsReversed);
                AddEdge(newEdge);
            }
        }

        private LayoutEdge CreateLayoutEdge(IEdge<ISized> originalEdge)
        {
            var sourceLayoutVertex = _originalToLayoutVertexMap[originalEdge.Source];
            var targetLayoutVertex = _originalToLayoutVertexMap[originalEdge.Target];
            return new LayoutEdge(originalEdge, sourceLayoutVertex, targetLayoutVertex);
        }

        private void Normalize()
        {
            RemoveLoops();
            RemoveCyclesByReversingEdges();
            //TODO? Remove multi-edges? Which instances?
        }

        private void RemoveLoops()
        {
            RemoveEdgeIf(i => i.Source == i.Target);
        }

        private void RemoveCyclesByReversingEdges()
        {
            // TODO? Reverse those edges that participate in many cycles.

            var cycleEdges = new List<LayoutEdge>();

            var searchAlgorithm = new DepthFirstSearchAlgorithm<LayoutVertex, LayoutEdge>(this);
            searchAlgorithm.BackEdge += cycleEdges.Add;
            searchAlgorithm.Compute();

            foreach (var edge in cycleEdges)
            {
                RemoveEdge(edge);
                AddEdge(edge.Reverse());
            }
        }
    }
}
