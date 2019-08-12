using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama.Absolute;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama.Relative.Logic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;

namespace Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama
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
            var diagramNodeToLayoutVertexMap = new Map<IDiagramNode, DiagramNodeLayoutVertex>(new DiagramNodeIdEqualityComparer());
            var diagramConnectorToLayoutPathMap = new Map<IDiagramConnector, LayoutPath>(new DiagramConnectorIdEqualityComparer());

            var layoutVertices = nodes.Select(i => CreateLayoutVertex(i, diagramNodeToLayoutVertexMap)).OrderBy(i => i.DiagramNode.AddedAt).ThenBy(i => i.Name).ToList();
            var layoutPaths = connectors.Select(i => CreateLayoutPath(i, diagramNodeToLayoutVertexMap, diagramConnectorToLayoutPathMap)).ToList();

            var relativeLayout = RelativeLayoutCalculator.Calculate(layoutVertices, layoutPaths);
            var layoutVertexToPointMap = AbsolutePositionCalculator.GetVertexCenters(relativeLayout, HorizontalGap, VerticalGap);

            return diagramNodeToLayoutVertexMap.ToDictionary(i => i.Key.Id, i => Rect2D.CreateFromCenterAndSize(layoutVertexToPointMap.Get(i.Value), i.Value.Size));
        }

        private DiagramNodeLayoutVertex CreateLayoutVertex(IDiagramNode diagramNode, Map<IDiagramNode, DiagramNodeLayoutVertex> diagramNodeToLayoutVertexMap)
        {
            if (diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var diagramNodeLayoutPriority = _layoutPriorityProvider.GetPriority(diagramNode);
            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode, diagramNode.Name, diagramNodeLayoutPriority);
            diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            return diagramNodeLayoutVertex;
        }

        private LayoutPath CreateLayoutPath(IDiagramConnector diagramConnector, Map<IDiagramNode, DiagramNodeLayoutVertex> diagramNodeToLayoutVertexMap, Map<IDiagramConnector, LayoutPath> diagramConnectorToLayoutPathMap)
        {
            if (diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var sourceVertex = diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            var layoutPath = new LayoutPath(sourceVertex, targetVertex, diagramConnector);

            diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            return layoutPath;
        }
    }
}