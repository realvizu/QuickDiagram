using System;

namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// A relatinship type with a direction.
    /// </summary>
    public struct DirectedModelRelationshipType
    {
        public Type Type { get; }
        public RelationshipDirection Direction { get; }

        public DirectedModelRelationshipType(Type type, RelationshipDirection direction)
        {
            Type = type;
            Direction = direction;
        }
    }
}
