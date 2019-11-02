using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Creates view models for roslyn-based diagram shapes.
    /// </summary>
    public sealed class RoslynDiagramShapeUiFactory : DiagramShapeUiFactory, IRoslynDiagramShapeUiFactory
    {
        public bool IsDescriptionVisible { get; set; }

        public RoslynDiagramShapeUiFactory(
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            bool isDescriptionVisible)
            : base(relatedNodeTypeProvider)
        {
            IsDescriptionVisible = isDescriptionVisible;
        }

        public override IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramService diagramService,
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new RoslynDiagramNodeViewModel(
                ModelService,
                diagramService,
                RelatedNodeTypeProvider,
                focusTracker,
                diagramNode,
                IsDescriptionVisible);
        }
    }
}