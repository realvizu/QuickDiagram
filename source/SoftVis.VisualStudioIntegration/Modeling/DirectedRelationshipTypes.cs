using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the directed relationship types.
    /// </summary>
    public static class DirectedRelationshipTypes
    {
        public static readonly DirectedModelRelationshipType BaseType = 
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotype.Inheritance, RelationshipDirection.Outgoing);

        public static readonly DirectedModelRelationshipType Subtype =
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotype.Inheritance, RelationshipDirection.Incoming);

        public static readonly DirectedModelRelationshipType ImplementedInterface =
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotype.Implementation, RelationshipDirection.Outgoing);

        public static readonly DirectedModelRelationshipType ImplementerType =
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotype.Implementation, RelationshipDirection.Incoming);

    }
}
