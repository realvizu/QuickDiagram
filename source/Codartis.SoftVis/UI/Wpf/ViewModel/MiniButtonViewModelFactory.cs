using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public sealed class MiniButtonViewModelFactory : IMiniButtonFactory
    {
        [NotNull] private readonly IModelEventSource _modelEventSource;
        [NotNull] private readonly IDiagramEventSource _diagramEventSource;
        [NotNull] private readonly IRelatedNodeTypeProvider _relatedNodeTypeProvider;

        public MiniButtonViewModelFactory(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider)
        {
            _modelEventSource = modelEventSource;
            _diagramEventSource = diagramEventSource;
            _relatedNodeTypeProvider = relatedNodeTypeProvider;
        }

        public IEnumerable<IMiniButton> CreateForDiagramShape(IDiagramShapeUi diagramShapeUi)
        {
            if (!(diagramShapeUi is IDiagramNodeUi diagramNodeUi))
                yield break;

            yield return new CloseMiniButtonViewModel(_modelEventSource, _diagramEventSource);

            foreach (var entityRelationType in _relatedNodeTypeProvider.GetRelatedNodeTypes(diagramNodeUi.DiagramNode.ModelNode.Stereotype))
                yield return new RelatedNodeMiniButtonViewModel(_modelEventSource, _diagramEventSource, entityRelationType);
        }
    }
}