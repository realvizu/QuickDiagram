using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using QuickGraph;
using QuickGraph.Algorithms.Search;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    /// <summary>
    /// A graph data structure used in Sugiyama layout algorithm.
    /// It's vertices and edges are of type SugiVertex and SugiEdge.
    /// It is always created for an original graph that is the subject of the layout process.
    /// </summary>
    internal sealed class SugiGraph : BidirectionalGraph<SugiVertex, SugiEdge>
    {
        private readonly IDictionary<ISized, SugiVertex> _originalToSugiVertexMap;

        internal SugiGraph(IEnumerable<ISized> originalVertices, IEnumerable<IEdge<ISized>> originalEdges)
        {
            var originalVerticesList = originalVertices as IList<ISized> ?? originalVertices.ToList();

            _originalToSugiVertexMap = originalVerticesList.ToDictionary(i => i, CreateSugiVertex);

            BuildSugiGraph(originalVerticesList, originalEdges);
        }

        private static SugiVertex CreateSugiVertex(ISized originalVertex)
        {
            return SugiVertex.CreateNormal(originalVertex, originalVertex.Size);
        }

        private void BuildSugiGraph(IEnumerable<ISized> originalVertices, IEnumerable<IEdge<ISized>> originalEdges)
        {
            foreach (var originalVertex in originalVertices)
                AddVertex(_originalToSugiVertexMap[originalVertex]);
            
            foreach (var originalEdge in originalEdges)
                AddEdge(CreateSugiEdge(originalEdge));
        }

        private SugiEdge CreateSugiEdge(IEdge<ISized> originalEdge)
        {
            var sourceSugiVertex = GetSugiVertexByOriginal(originalEdge.Source);
            var targetSugiVertex = GetSugiVertexByOriginal(originalEdge.Target);

            return new SugiEdge(originalEdge, sourceSugiVertex, targetSugiVertex);
        }

        public void BreakEdgeWithDummyVertex(SugiEdge sugiEdge, SugiVertex rVertex)
        {
            RemoveEdge(sugiEdge);

            AddVertex(rVertex);
            AddEdge(new SugiEdge(sugiEdge.OriginalEdge, sugiEdge.Source, rVertex));
            AddEdge(new SugiEdge(sugiEdge.OriginalEdge, rVertex, sugiEdge.Target));
        }

        public void BreakEdgeWithSegment(SugiEdge sugiEdge, Segment segment)
        {
            RemoveEdge(sugiEdge);

            var pVertex = segment.PVertex;
            var qVertex = segment.QVertex;

            AddVertex(pVertex);
            AddVertex(qVertex);
            AddEdge(new SugiEdge(sugiEdge.OriginalEdge, sugiEdge.Source, pVertex));
            AddEdge(new SugiEdge(sugiEdge.OriginalEdge, pVertex, qVertex));
            AddEdge(new SugiEdge(sugiEdge.OriginalEdge, qVertex, sugiEdge.Target));
        }

        public SugiVertex GetSugiVertexByOriginal(ISized originalVertex)
        {
            return _originalToSugiVertexMap[originalVertex];
        }

        public Point2D GetNewCenter(ISized originalVertex)
        {
            return GetSugiVertexByOriginal(originalVertex).Center;
        }

        public IEnumerable<SugiVertex> GetIsolatedVertices()
        {
            return Vertices.Where(i => Degree(i) == 0);
        }

        public void RemoveVertices(IEnumerable<SugiVertex> sugiVertices)
        {
            foreach (var sugiVertex in sugiVertices)
                RemoveVertex(sugiVertex);
        }

        /// <summary>
        /// Removes the edges which source and target is the same vertex.
        /// </summary>
        public void RemoveLoops()
        {
            RemoveEdgeIf(edge => edge.Source == edge.Target);
        }

        /// <summary>
        /// Removes the cycles from the graph with simply reverting some edges.
        /// </summary>
        public void RemoveCycles()
        {
            var cycleEdges = new List<SugiEdge>();

            var searchAlgorithm = new DepthFirstSearchAlgorithm<SugiVertex, SugiEdge>(this);
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