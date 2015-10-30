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
        private readonly LayeringGraph _layeringGraph;
        private readonly Map<DiagramNode, LayeringVertex> _diagramNodeToLayeringVertexMap;
        private readonly Map<DiagramConnector, LayeringEdge> _diagramConnectorToLayeringEdgeMap;

        public DiagramNodeRankCalculator()
        {
            _layeringGraph = new LayeringGraph();
            _diagramNodeToLayeringVertexMap = new Map<DiagramNode, LayeringVertex>();
            _diagramConnectorToLayeringEdgeMap = new Map<DiagramConnector, LayeringEdge>();
        }

        public void Clear()
        {
            _layeringGraph.Clear();
            _diagramNodeToLayeringVertexMap.Clear();
            _diagramConnectorToLayeringEdgeMap.Clear();
        }

        public void Add(DiagramNode diagramNode)
        {
            var layeringVertex = new LayeringVertex(diagramNode);
            _layeringGraph.AddVertex(layeringVertex);
            _diagramNodeToLayeringVertexMap.Set(diagramNode, layeringVertex);
        }

        public void Remove(DiagramNode diagramNode)
        {
            var layeringVertex = _diagramNodeToLayeringVertexMap.Get(diagramNode);
            _layeringGraph.RemoveVertex(layeringVertex);
            _diagramNodeToLayeringVertexMap.Remove(diagramNode);
        }

        public void Add(DiagramConnector diagramConnector)
        {
            var sourceLayeringVertex = _diagramNodeToLayeringVertexMap.Get(diagramConnector.Source);
            var targetLayeringVertex = _diagramNodeToLayeringVertexMap.Get(diagramConnector.Target);
            var layeringEdge = new LayeringEdge(sourceLayeringVertex, targetLayeringVertex);
            _layeringGraph.AddEdge(layeringEdge);
            _diagramConnectorToLayeringEdgeMap.Set(diagramConnector, layeringEdge);

            UpdateLayerIndexesRecursive(sourceLayeringVertex);
        }

        public void Remove(DiagramConnector diagramConnector)
        {
            var layeringEdge = _diagramConnectorToLayeringEdgeMap.Get(diagramConnector);
            _layeringGraph.RemoveEdge(layeringEdge);
            _diagramConnectorToLayeringEdgeMap.Remove(diagramConnector);
        }

        public int GetRank(DiagramNode diagramNode)
        {
            return _diagramNodeToLayeringVertexMap.Get(diagramNode).LayerIndex;
        }

        public int GetRankSpan(DiagramConnector diagramConnector)
        {
            return _diagramConnectorToLayeringEdgeMap.Get(diagramConnector).LayerSpan;
        }

        private void UpdateLayerIndexesRecursive(LayeringVertex updateRootLayeringVertex)
        {
            _layeringGraph.ExecuteOnVerticesRecursive(updateRootLayeringVertex, EdgeDirection.In, UpdateLayerIndex);
        }

        private void UpdateLayerIndex(LayeringVertex layeringVertex)
        {
            layeringVertex.LayerIndex = CalculateLayerIndex(layeringVertex);
        }

        private int CalculateLayerIndex(LayeringVertex layeringVertex)
        {
            return _layeringGraph.OutEdges(layeringVertex).Select(i => i.Target.LayerIndex).DefaultIfEmpty(-1).Max() + 1;
        }
    }
}
