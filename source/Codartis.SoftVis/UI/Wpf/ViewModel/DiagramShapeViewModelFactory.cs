using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Default implementation of a factory that create view models from diagram shapes.
    /// </summary>
    public class DiagramShapeViewModelFactory : IDiagramShapeUiFactory
    {
        [NotNull] protected IModelEventSource ModelEventSource { get; }
        [NotNull] protected IDiagramEventSource DiagramEventSource { get; }
        [NotNull] protected IRelatedNodeTypeProvider RelatedNodeTypeProvider { get; }

        public DiagramShapeViewModelFactory(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider)
        {
            ModelEventSource = modelEventSource;
            DiagramEventSource = diagramEventSource;
            RelatedNodeTypeProvider = relatedNodeTypeProvider;
        }

        public virtual IDiagramNodeUi CreateDiagramNodeUi(IDiagramNode diagramNode)
        {
            return new DiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramNode,
                RelatedNodeTypeProvider,
                new DiagramNodeHeaderViewModel { Payload = diagramNode.ModelNode.Payload });
        }

        public virtual IDiagramConnectorUi CreateDiagramConnectorUi(IDiagramConnector diagramConnector)
        {
            return new DiagramConnectorViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramConnector);
        }
    }
}