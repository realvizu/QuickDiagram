using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Listens to diagram modification events and performs layout.
    /// </summary>
    public sealed class AutoLayoutDiagramPlugin : DiagramPluginBase
    {
        private static readonly TimeSpan DiagramEventDebounceTimeSpan = TimeSpan.FromMilliseconds(100);

        private static readonly DiagramNodeMember[] DiagramMembersAffectedByLayout =
        {
            DiagramNodeMember.RelativePosition,
            DiagramNodeMember.AbsolutePosition
        };

        [NotNull] private readonly ILayoutAlgorithmSelectionStrategy _layoutAlgorithmSelectionStrategy;
        [NotNull] private readonly IConnectorRoutingAlgorithm _crossLayoutGroupConnectorRoutingAlgorithm;
        [NotNull] private readonly IDisposable _diagramChangedSubscription;

        public AutoLayoutDiagramPlugin(
            [NotNull] IDiagramService diagramService,
            [NotNull] ILayoutAlgorithmSelectionStrategy layoutAlgorithmSelectionStrategy,
            [NotNull] IConnectorRoutingAlgorithm crossLayoutGroupConnectorRoutingAlgorithm)
            : base(diagramService)
        {
            _layoutAlgorithmSelectionStrategy = layoutAlgorithmSelectionStrategy;
            _crossLayoutGroupConnectorRoutingAlgorithm = crossLayoutGroupConnectorRoutingAlgorithm;

            _diagramChangedSubscription = CreateDiagramChangedSubscription();
        }

        public override void Dispose()
        {
            _diagramChangedSubscription.Dispose();
        }

        [NotNull]
        private IDisposable CreateDiagramChangedSubscription()
        {
            // Filter on layout triggering events, group them by parent and debounce each group individually.

            return DiagramService.DiagramChangedEventStream
                .SelectMany(i => i.ShapeEvents, (diagramEvent, shapeEvent) => (diagramEvent.OldDiagram, diagramEvent.NewDiagram, shapeEvent))
                .Where(i => IsLayoutTriggeringChange(i.shapeEvent))
                .GroupBy(i => GetParentId(i.OldDiagram, i.NewDiagram, i.shapeEvent))
                .Subscribe(
                    group =>
                        group
                            .Throttle(DiagramEventDebounceTimeSpan)
                            .Subscribe(i => OnDiagramChanged(i.OldDiagram, i.NewDiagram, i.shapeEvent)));
        }

        private static ModelNodeId? GetParentId(
            [NotNull] IDiagram oldDiagram,
            [NotNull] IDiagram newDiagram,
            [NotNull] DiagramShapeEventBase diagramShapeEvent)
        {
            return diagramShapeEvent switch
            {
                DiagramNodeAddedEvent i => GetParentId(i.NewNode),
                DiagramNodeChangedEvent i => GetParentId(i.NewNode),
                DiagramNodeRemovedEvent i => GetParentId(i.OldNode),
                DiagramConnectorAddedEvent i => GetParentId(i.NewConnector, newDiagram),
                DiagramConnectorRouteChangedEvent i => GetParentId(i.NewConnector, newDiagram),
                DiagramConnectorRemovedEvent i => GetParentId(i.OldConnector, oldDiagram),
                _ => throw new Exception($"Unexpected DiagramShapeEvent: {diagramShapeEvent}")
            };
        }

        private static ModelNodeId? GetParentId([NotNull] IDiagramNode diagramNode) => diagramNode.ParentNodeId.ToNullable();

        private static ModelNodeId? GetParentId([NotNull] IDiagramConnector diagramConnector, [NotNull] IDiagram diagram)
        {
            var sourceNode = diagram.GetNode(diagramConnector.Source);
            var targetNode = diagram.GetNode(diagramConnector.Target);

            return sourceNode.ParentNodeId == targetNode.ParentNodeId
                ? sourceNode.ParentNodeId.ToNullable()
                : null;
        }

        private void OnDiagramChanged([NotNull] IDiagram oldDiagram, [NotNull] IDiagram newDiagram, [NotNull] DiagramShapeEventBase diagramShapeEvent)
        {
            Debug.WriteLine($"DiagramShapeEvent={diagramShapeEvent}");

            LayoutParentGroup(oldDiagram, newDiagram, diagramShapeEvent);

            LayoutCrossGroupConnectors(DiagramService.LatestDiagram);

            // This is a bit vague to lay out all cross-group connectors after any group layout.
            // A better approach could be to create a separate subscription
            // that reacts to cross-group connectors' source/target node absolute position change (optionally with debounce).
        }

        private void LayoutParentGroup(
            [NotNull] IDiagram oldDiagram,
            [NotNull] IDiagram newDiagram,
            [NotNull] DiagramShapeEventBase diagramShapeEvent)
        {
            IGroupLayoutAlgorithm layoutAlgorithm;

            var parentId = GetParentId(oldDiagram, newDiagram, diagramShapeEvent);
            if (parentId == null)
            {
                layoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForRoot();
            }
            else
            {
                if (!newDiagram.NodeExists(parentId.Value))
                    return;

                layoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForNode(newDiagram.GetNode(parentId.Value));
            }

            // BUGBUG: Until a better layout algorithm is implemented all connectors that might cause a loop are treated as cross-layout-group connectors.
            var layoutGroup = newDiagram.CreateLayoutGroup(parentId.ToMaybe(), i => !CanCauseLoop(i));
            if (layoutGroup.IsEmpty)
                return;

            var layoutInfo = layoutAlgorithm.Calculate(layoutGroup);

            DiagramService.ApplyLayout(layoutInfo);
        }

        private void LayoutCrossGroupConnectors([NotNull] IDiagram diagram)
        {
            var connectors = diagram.GetCrossLayoutGroupConnectors()
                // BUGBUG: Until a better layout algorithm is implemented all connectors that might cause a loop are treated as cross-layout-group connectors.
                .Union(diagram.Connectors.Where(CanCauseLoop));

            var nodeRects = diagram.Nodes.Where(i => i.AbsoluteRect.IsDefined()).ToDictionary(i => i.Id, i => i.AbsoluteRect);

            var connectorRoutes = _crossLayoutGroupConnectorRoutingAlgorithm.Calculate(connectors, nodeRects);
            var layoutInfo = new LayoutInfo(connectorRoutes);
            DiagramService.ApplyLayout(layoutInfo);
        }

        private static bool CanCauseLoop([NotNull] IDiagramConnector connector) => connector.ModelRelationship.Stereotype.Name == "Association";

        private static bool IsLayoutTriggeringChange(DiagramShapeEventBase diagramShapeEvent)
        {
            switch (diagramShapeEvent)
            {
                case DiagramNodeChangedEvent nodeChangedEvent when nodeChangedEvent.ChangedMember.In(DiagramMembersAffectedByLayout):
                case DiagramConnectorRouteChangedEvent _:
                    return false;

                default:
                    return true;
            }
        }
    }
}