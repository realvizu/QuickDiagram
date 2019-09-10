using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Layout.Sugiyama.Absolute;
using Codartis.SoftVis.Diagramming.Layout.Sugiyama.Relative.Logic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;

namespace Codartis.SoftVis.Diagramming.Layout.Sugiyama
{
    public sealed class SugiyamaLayoutAlgorithm : INodeLayoutAlgorithm
    {
        private const double HorizontalGap = DiagramLayoutDefaults.HorizontalGap;
        private const double VerticalGap = DiagramLayoutDefaults.VerticalGap;

        private readonly ILayoutPriorityProvider _layoutPriorityProvider;

        public SugiyamaLayoutAlgorithm(ILayoutPriorityProvider layoutPriorityProvider)
        {
            _layoutPriorityProvider = layoutPriorityProvider;
        }

        public IDictionary<ModelNodeId, Rect2D> Calculate(IEnumerable<IDiagramNode> nodes, IEnumerable<IDiagramConnector> connectors)
        {
            var diagramNodeToLayoutVertexMap = new Map<ModelNodeId, DiagramNodeLayoutVertex>();
            var diagramConnectorToLayoutPathMap = new Map<ModelRelationshipId, LayoutPath>();

            var layoutVertices = nodes.Select(i => CreateLayoutVertex(i, diagramNodeToLayoutVertexMap)).OrderBy(i => i.DiagramNode.AddedAt).ThenBy(i => i.Name)
                .ToList();
            var layoutPaths = connectors.Select(i => CreateLayoutPath(i, diagramNodeToLayoutVertexMap, diagramConnectorToLayoutPathMap)).ToList();

            var relativeLayout = RelativeLayoutCalculator.Calculate(layoutVertices, layoutPaths);
            var layoutVertexToPointMap = AbsolutePositionCalculator.GetVertexCenters(relativeLayout, HorizontalGap, VerticalGap);

            return diagramNodeToLayoutVertexMap.ToDictionary(
                i => i.Key,
                i => Rect2D.CreateFromCenterAndSize(layoutVertexToPointMap.Get(i.Value), i.Value.Size));
        }

        private DiagramNodeLayoutVertex CreateLayoutVertex(IDiagramNode diagramNode, Map<ModelNodeId, DiagramNodeLayoutVertex> diagramNodeToLayoutVertexMap)
        {
            if (diagramNodeToLayoutVertexMap.Contains(diagramNode.Id))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var diagramNodeLayoutPriority = _layoutPriorityProvider.GetPriority(diagramNode);
            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode, diagramNodeLayoutPriority);
            diagramNodeToLayoutVertexMap.Set(diagramNode.Id, diagramNodeLayoutVertex);

            return diagramNodeLayoutVertex;
        }

        private LayoutPath CreateLayoutPath(
            IDiagramConnector diagramConnector,
            Map<ModelNodeId, DiagramNodeLayoutVertex> diagramNodeToLayoutVertexMap,
            Map<ModelRelationshipId, LayoutPath> diagramConnectorToLayoutPathMap)
        {
            if (diagramConnectorToLayoutPathMap.Contains(diagramConnector.Id))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var sourceVertex = diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            var layoutPath = new LayoutPath(sourceVertex, targetVertex, diagramConnector);

            diagramConnectorToLayoutPathMap.Set(diagramConnector.Id, layoutPath);

            return layoutPath;
        }
    }
}