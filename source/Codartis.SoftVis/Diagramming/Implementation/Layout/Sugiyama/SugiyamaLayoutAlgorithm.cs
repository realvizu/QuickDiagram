using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Absolute;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama
{
    public sealed class SugiyamaLayoutAlgorithm : IGroupLayoutAlgorithm
    {
        private const double HorizontalGap = DiagramLayoutDefaults.HorizontalGap;
        private const double VerticalGap = DiagramLayoutDefaults.VerticalGap;

        [NotNull] private readonly ILayoutPriorityProvider _layoutPriorityProvider;

        public SugiyamaLayoutAlgorithm([NotNull] ILayoutPriorityProvider layoutPriorityProvider)
        {
            _layoutPriorityProvider = layoutPriorityProvider;
        }

        public LayoutInfo Calculate(ILayoutGroup layoutGroup)
        {
            var diagramNodeToLayoutVertexMap = new Map<ModelNodeId, DiagramNodeLayoutVertex>();
            var diagramConnectorToLayoutPathMap = new Map<ModelRelationshipId, LayoutPath>();

            var layoutVertices = layoutGroup.Nodes
                .Select(i => CreateLayoutVertex(i, diagramNodeToLayoutVertexMap))
                .OrderBy(i => i.DiagramNode.AddedAt)
                .ThenBy(i => i.Name)
                .ToList();

            var layoutPaths = layoutGroup.Connectors
                .Select(i => CreateLayoutPath(i, diagramNodeToLayoutVertexMap, diagramConnectorToLayoutPathMap))
                .ToList();

            var relativeLayout = RelativeLayoutCalculator.Calculate(layoutVertices, layoutPaths);

            return new AbsolutePositionCalculator(relativeLayout, HorizontalGap, VerticalGap).CalculateLayout();
        }

        [NotNull]
        private DiagramNodeLayoutVertex CreateLayoutVertex(
            [NotNull] IDiagramNode diagramNode,
            [NotNull] Map<ModelNodeId, DiagramNodeLayoutVertex> diagramNodeToLayoutVertexMap)
        {
            if (diagramNodeToLayoutVertexMap.Contains(diagramNode.Id))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var diagramNodeLayoutPriority = _layoutPriorityProvider.GetPriority(diagramNode);
            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode, diagramNodeLayoutPriority);
            diagramNodeToLayoutVertexMap.Set(diagramNode.Id, diagramNodeLayoutVertex);

            return diagramNodeLayoutVertex;
        }

        [NotNull]
        private static LayoutPath CreateLayoutPath(
            [NotNull] IDiagramConnector diagramConnector,
            [NotNull] Map<ModelNodeId, DiagramNodeLayoutVertex> diagramNodeToLayoutVertexMap,
            [NotNull] Map<ModelRelationshipId, LayoutPath> diagramConnectorToLayoutPathMap)
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