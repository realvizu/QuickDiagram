using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    /// <summary>
    /// Defines the directed relationship types.
    /// </summary>
    public static class DirectedRelationshipTypes
    {
        public static readonly DirectedModelRelationshipType BaseType = 
            new DirectedModelRelationshipType(typeof(InheritanceRelationship), RelationshipDirection.Outgoing);

        public static readonly DirectedModelRelationshipType Subtype =
            new DirectedModelRelationshipType(typeof(InheritanceRelationship), RelationshipDirection.Incoming);

        public static readonly DirectedModelRelationshipType ImplementedInterface =
            new DirectedModelRelationshipType(typeof(ImplementsRelationship), RelationshipDirection.Outgoing);

        public static readonly DirectedModelRelationshipType ImplementerType =
            new DirectedModelRelationshipType(typeof(ImplementsRelationship), RelationshipDirection.Incoming);

    }
}
