using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the directed relationship types.
    /// </summary>
    public static class DirectedModelRelationshipTypes
    {
        public static readonly DirectedModelRelationshipType BaseType = 
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Inheritance, EdgeDirection.Out);
        
        public static readonly DirectedModelRelationshipType Subtype =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Inheritance, EdgeDirection.In);

        public static readonly DirectedModelRelationshipType ImplementedInterface =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Implementation, EdgeDirection.Out);

        public static readonly DirectedModelRelationshipType ImplementerType =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Implementation, EdgeDirection.In);
        
        public static readonly DirectedModelRelationshipType AssociatedType =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Association, EdgeDirection.Out);

        public static readonly DirectedModelRelationshipType AssociatingType =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Association, EdgeDirection.In);

        public static readonly DirectedModelRelationshipType Member =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Member, EdgeDirection.Out);

        public static readonly DirectedModelRelationshipType DeclaringType =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Member, EdgeDirection.In);
    }
}
