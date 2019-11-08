using System;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Base class for factories that create view models from diagram shapes.
    /// </summary>
    public class DiagramShapeUiFactory : IDiagramShapeUiFactory
    {
        [NotNull] protected IRelatedNodeTypeProvider RelatedNodeTypeProvider { get; }
        protected IModelService ModelService { get; private set; }
        protected IDiagramShapeUiRepository DiagramShapeUiRepository { get; private set; }

        public DiagramShapeUiFactory([NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider)
        {
            RelatedNodeTypeProvider = relatedNodeTypeProvider;
        }

        // TODO: merge into ctor
        public void Initialize([NotNull] IModelService modelService, [NotNull] IDiagramShapeUiRepository diagramShapeUiRepository)
        {
            ModelService = modelService;
            DiagramShapeUiRepository = diagramShapeUiRepository;
        }

        [NotNull]
        public virtual IDiagramNodeUi CreateDiagramNodeUi(
            [NotNull] IDiagramService diagramService,
            [NotNull] IDiagramNode diagramNode,
            [NotNull] IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new DiagramNodeViewModel(ModelService, diagramService, RelatedNodeTypeProvider, focusTracker, diagramNode);
        }

        [NotNull]
        public virtual IDiagramConnectorUi CreateDiagramConnectorUi([NotNull] IDiagramService diagramService, [NotNull] IDiagramConnector diagramConnector)
        {
            return new DiagramConnectorViewModel(
                ModelService,
                diagramService,
                diagramConnector);
        }
    }
}