using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Creates view models for roslyn-based diagram shapes.
    /// </summary>
    public sealed class RoslynDiagramShapeViewModelFactory : DiagramShapeViewModelFactory, IRoslynDiagramShapeUiFactory
    {
        public bool IsDescriptionVisible { get; set; }

        public RoslynDiagramShapeViewModelFactory(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            bool isDescriptionVisible)
            : base(modelEventSource, diagramEventSource, relatedNodeTypeProvider)
        {
            IsDescriptionVisible = isDescriptionVisible;
        }

        public override IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new RoslynDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramNode,
                PayloadUiFactory?.Create(diagramNode.ModelNode.Payload),
                RelatedNodeTypeProvider,
                (IWpfFocusTracker<IDiagramShapeUi>)focusTracker,
                IsDescriptionVisible);
        }
    }
}