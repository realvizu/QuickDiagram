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
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotypes.Inheritance, EdgeDirection.Out);
        
        public static readonly DirectedModelRelationshipType Subtype =
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotypes.Inheritance, EdgeDirection.In);

        public static readonly DirectedModelRelationshipType ImplementedInterface =
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotypes.Implementation, EdgeDirection.Out);

        public static readonly DirectedModelRelationshipType ImplementerType =
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotypes.Implementation, EdgeDirection.In);
        
        public static readonly DirectedModelRelationshipType AssociatedType =
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotypes.Association, EdgeDirection.Out);

        public static readonly DirectedModelRelationshipType AssociatingType =
            new DirectedModelRelationshipType(RoslynModelRelationshipStereotypes.Association, EdgeDirection.In);

    }
}
