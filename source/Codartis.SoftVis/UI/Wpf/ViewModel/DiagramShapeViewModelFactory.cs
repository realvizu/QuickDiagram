using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Base class for factories that create view models from diagram shapes.
    /// </summary>
    public class DiagramShapeViewModelFactory : IDiagramShapeUiFactory
    {
        [NotNull] protected IModelService ModelService { get; }
        [NotNull] protected IDiagramService DiagramService { get; }
        [NotNull] protected IRelatedNodeTypeProvider RelatedNodeTypeProvider { get; }

        public DiagramShapeViewModelFactory(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider)
        {
            ModelService = modelService;
            DiagramService = diagramService;
            RelatedNodeTypeProvider = relatedNodeTypeProvider;
        }

        public virtual IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new DiagramNodeViewModel(
                ModelService,
                DiagramService,
                RelatedNodeTypeProvider,
                (IWpfFocusTracker<IDiagramShapeUi>)focusTracker,
                diagramNode);
        }

        public virtual IDiagramConnectorUi CreateDiagramConnectorUi(
            IDiagramConnector diagramConnector,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new DiagramConnectorViewModel(
                ModelService,
                DiagramService,
                diagramConnector);
        }
    }
}