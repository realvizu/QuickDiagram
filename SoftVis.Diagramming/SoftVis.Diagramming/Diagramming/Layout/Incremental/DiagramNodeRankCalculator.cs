using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates diagram node rank when nodes and connectors are added or removed.
    /// </summary>
    /// <remarks>
    /// Ranking rule: for every connector: connector.Source.Rank > connector.Target.Rank
    /// </remarks>
    internal sealed class DiagramNodeRankCalculator : IDiagramChangeConsumer, IDiagramNodeRankProvider
    {
        private readonly RankingGraph _rankingGraph;
        private readonly Map<DiagramNode, RankingVertex> _diagramNodeToRankingVertexMap;
        private readonly Map<DiagramConnector, RankingEdge> _diagramConnectorToRankingEdgeMap;

        public DiagramNodeRankCalculator()
        {
            _rankingGraph = new RankingGraph();
            _diagramNodeToRankingVertexMap = new Map<DiagramNode, RankingVertex>();
            _diagramConnectorToRankingEdgeMap = new Map<DiagramConnector, RankingEdge>();
        }

        public void Clear()
        {
            _rankingGraph.Clear();
            _diagramNodeToRankingVertexMap.Clear();
            _diagramConnectorToRankingEdgeMap.Clear();
        }

        public void Add(DiagramNode diagramNode)
        {
            var rankingVertex = new RankingVertex(diagramNode);
            _rankingGraph.AddVertex(rankingVertex);
            _diagramNodeToRankingVertexMap.Set(diagramNode, rankingVertex);
        }

        public void Remove(DiagramNode diagramNode)
        {
            var rankingVertex = _diagramNodeToRankingVertexMap.Get(diagramNode);
            _rankingGraph.RemoveVertex(rankingVertex);
            _diagramNodeToRankingVertexMap.Remove(diagramNode);
        }

        public void Add(DiagramConnector diagramConnector)
        {
            var sourceRankingVertex = _diagramNodeToRankingVertexMap.Get(diagramConnector.Source);
            var targetRankingVertex = _diagramNodeToRankingVertexMap.Get(diagramConnector.Target);
            var rankingEdge = new RankingEdge(sourceRankingVertex, targetRankingVertex);
            _rankingGraph.AddEdge(rankingEdge);
            _diagramConnectorToRankingEdgeMap.Set(diagramConnector, rankingEdge);

            UpdateRanksRecursive(sourceRankingVertex);
        }

        public void Remove(DiagramConnector diagramConnector)
        {
            var rankingEdge = _diagramConnectorToRankingEdgeMap.Get(diagramConnector);
            _rankingGraph.RemoveEdge(rankingEdge);
            _diagramConnectorToRankingEdgeMap.Remove(diagramConnector);
        }

        public int GetRank(DiagramNode diagramNode)
        {
            return _diagramNodeToRankingVertexMap.Get(diagramNode).Rank;
        }

        public int GetRankSpan(DiagramConnector diagramConnector)
        {
            return _diagramConnectorToRankingEdgeMap.Get(diagramConnector).RankSpan;
        }

        private void UpdateRanksRecursive(RankingVertex updateRootRankingVertex)
        {
            _rankingGraph.ExecuteOnVerticesRecursive(updateRootRankingVertex, EdgeDirection.In, UpdateRank);
        }

        private void UpdateRank(RankingVertex rankingVertex)
        {
            rankingVertex.Rank = CalculateRank(rankingVertex);
        }

        private int CalculateRank(RankingVertex rankingVertex)
        {
            return _rankingGraph.OutEdges(rankingVertex).Select(i => i.Target.Rank).DefaultIfEmpty(-1).Max() + 1;
        }
    }
}
