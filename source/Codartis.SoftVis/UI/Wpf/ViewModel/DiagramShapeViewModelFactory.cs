using System.Collections.Generic;
using System.Linq;
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
                CreateHeaderUi(diagramNode),
                CreateRelatedNodeCueViewModels(diagramNode));
        }

        public virtual IDiagramConnectorUi CreateDiagramConnectorUi(IDiagramConnector diagramConnector)
        {
            return new DiagramConnectorViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramConnector);
        }

        [NotNull]
        [ItemNotNull]
        protected List<RelatedNodeCueViewModel> CreateRelatedNodeCueViewModels([NotNull] IDiagramNode diagramNode)
        {
            return RelatedNodeTypeProvider.GetRelatedNodeTypes(diagramNode.ModelNode.Stereotype)
                .Select(i => new RelatedNodeCueViewModel(ModelEventSource, DiagramEventSource, diagramNode, i))
                .ToList();
        }

        [NotNull]
        private static DiagramNodeHeaderViewModel CreateHeaderUi([NotNull] IDiagramNode diagramNode)
        {
            return new DiagramNodeHeaderViewModel { Payload = diagramNode.ModelNode.Payload };
        }
    }
}