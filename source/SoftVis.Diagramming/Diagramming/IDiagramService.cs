using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Defines diagram-related operations.
    /// </summary>
    public interface IDiagramService : IDiagramMutator, IDiagramShapeResolver
    {
        IDiagramNode ShowModelNode(IModelNode modelNode);
        void HideModelNode(IModelNode modelNode);

        IReadOnlyList<IDiagramNode> ShowModelNodes(IEnumerable<IModelNode> modelNodes,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null);

        void ShowModelRelationship(IModelRelationship modelRelationship);
        void HideModelRelationship(IModelRelationship modelRelationship);

        Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds);
    }
}
