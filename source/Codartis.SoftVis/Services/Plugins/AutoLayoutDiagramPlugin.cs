using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Diagramming.Definition.Layout;
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
        private static readonly DiagramNodeMember[] DiagramMembersAffectedByLayout =
        {
            DiagramNodeMember.ChildrenAreaSize,
            DiagramNodeMember.Position
        };

        [NotNull] private readonly IDiagramLayoutAlgorithm _layoutAlgorithm;

        public AutoLayoutDiagramPlugin([NotNull] IDiagramLayoutAlgorithm layoutAlgorithm)
        {
            _layoutAlgorithm = layoutAlgorithm;
        }

        public override void Initialize(IModelService modelService, IDiagramService diagramService)
        {
            base.Initialize(modelService, diagramService);

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            DiagramService.DiagramChanged -= OnDiagramChanged;
        }

        private void OnDiagramChanged(DiagramEvent diagramEvent)
        {
            if (diagramEvent.ShapeEvents.All(i => !IsLayoutTriggeringChange(i)))
                return;

            var diagram = diagramEvent.NewDiagram;
            var diagramLayoutInfo = _layoutAlgorithm.Calculate(diagram);
            DiagramService.ApplyLayout(diagramLayoutInfo);
        }

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