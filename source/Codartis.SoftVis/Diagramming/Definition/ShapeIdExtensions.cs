using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public static class ShapeIdExtensions
    {
        [NotNull]
        public static string ToShapeId(this ModelNodeId nodeId) => nodeId.ToString();

        [NotNull]
        public static string ToShapeId(this ModelRelationshipId relationshipId) => relationshipId.ToString();
    }
}