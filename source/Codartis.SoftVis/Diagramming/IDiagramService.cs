using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.Util;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Defines diagram-related operations.
    /// </summary>
    public interface IDiagramService : IDiagramMutator, IDiagramShapeResolver
    {
        IDiagramNode ShowModelNode(IModelNode modelNode);
        void HideModelNode(ModelNodeId modelNodeId);

        IReadOnlyList<IDiagramNode> ShowModelNodes(IEnumerable<IModelNode> modelNodes,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);

        void ShowModelRelationship(IModelRelationship modelRelationship);
        void HideModelRelationship(ModelRelationshipId modelRelationshipId);

        bool TryGetContainerNode(IDiagramNode diagramNode, out IDiagramNode containerNode);

        Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds);
    }
}
