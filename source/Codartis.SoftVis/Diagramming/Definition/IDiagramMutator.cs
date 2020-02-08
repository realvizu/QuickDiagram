using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Mutates a diagram.
    /// </summary>
    /// <remarks>
    /// Absolute positions and ChildrenAreaSize cannot be mutated directly, they are calculated.
    /// </remarks>
    public interface IDiagramMutator
    {
        void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null);
        void UpdateNodeHeaderSize(ModelNodeId nodeId, Size2D newSize);
        void UpdateNodeRelativeTopLeft(ModelNodeId nodeId, Point2D newRelativeTopLeft);
        void RemoveNode(ModelNodeId nodeId);

        void AddConnector(ModelRelationshipId relationshipId);
        void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute);
        void RemoveConnector(ModelRelationshipId relationshipId);

        /// <remarks>
        /// This should remove all shapes whose model ID does not exist in the new model.
        /// </remarks>
        void UpdateModel([NotNull] IModel newModel);

        void UpdateModelNode([NotNull] IModelNode updatedModelNode);

        void ApplyLayout(LayoutInfo layoutInfo);

        void Clear();
    }
}