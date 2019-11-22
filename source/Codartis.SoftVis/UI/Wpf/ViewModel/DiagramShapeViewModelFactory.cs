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
        [NotNull] protected IModelEventSource ModelEventSource { get; }
        [NotNull] protected IDiagramEventSource DiagramEventSource { get; }
        [NotNull] protected IRelatedNodeTypeProvider RelatedNodeTypeProvider { get; }

        public IPayloadUiFactory PayloadUiFactory { get; }

        public DiagramShapeViewModelFactory(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [CanBeNull] IPayloadUiFactory payloadUiFactory = null)
        {
            ModelEventSource = modelEventSource;
            DiagramEventSource = diagramEventSource;
            RelatedNodeTypeProvider = relatedNodeTypeProvider;
            PayloadUiFactory = payloadUiFactory;
        }

        public virtual IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new DiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramNode,
                PayloadUiFactory?.Create(diagramNode.ModelNode.Payload),
                RelatedNodeTypeProvider,
                (IWpfFocusTracker<IDiagramShapeUi>)focusTracker);
        }

        public virtual IDiagramConnectorUi CreateDiagramConnectorUi(
            IDiagramConnector diagramConnector,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new DiagramConnectorViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramConnector,
                PayloadUiFactory?.Create(diagramConnector.ModelRelationship.Payload));
        }
    }
}