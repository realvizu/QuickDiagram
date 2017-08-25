using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the directed relationship types.
    /// </summary>
    public static class DirectedRelationshipTypes
    {
        public static readonly DirectedModelRelationshipType BaseType = 
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Inheritance, RelationshipDirection.Outgoing);

        public static readonly DirectedModelRelationshipType Subtype =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Inheritance, RelationshipDirection.Incoming);

        public static readonly DirectedModelRelationshipType ImplementedInterface =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Implementation, RelationshipDirection.Outgoing);

        public static readonly DirectedModelRelationshipType ImplementerType =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Implementation, RelationshipDirection.Incoming);

    }
}
