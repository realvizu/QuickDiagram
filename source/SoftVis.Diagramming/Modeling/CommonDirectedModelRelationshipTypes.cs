using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Defines directed relationship types.
    /// </summary>
    public static class CommonDirectedModelRelationshipTypes
    {
        public static readonly DirectedModelRelationshipType Container = 
            new DirectedModelRelationshipType(ModelRelationshipStereotype.Containment, EdgeDirection.In);

        public static readonly DirectedModelRelationshipType Contained =
            new DirectedModelRelationshipType(ModelRelationshipStereotype.Containment, EdgeDirection.Out);
    }
}
