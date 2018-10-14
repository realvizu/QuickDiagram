using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the directed relationship types.
    /// </summary>
    public static class DirectedRelationshipTypes
    {
        public static readonly DirectedModelRelationshipType BaseType = 
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Inheritance, EdgeDirection.Out);
        
        public static readonly DirectedModelRelationshipType Subtype =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Inheritance, EdgeDirection.In);

        public static readonly DirectedModelRelationshipType ImplementedInterface =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Implementation, EdgeDirection.Out);

        public static readonly DirectedModelRelationshipType ImplementerType =
            new DirectedModelRelationshipType(ModelRelationshipStereotypes.Implementation, EdgeDirection.In);

    }
}
