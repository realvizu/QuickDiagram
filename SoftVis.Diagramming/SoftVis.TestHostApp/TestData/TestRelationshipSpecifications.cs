using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal static class TestRelationshipSpecifications
    {
        public static readonly RelatedEntitySpecification ImplementedInterfaces = new RelatedEntitySpecification(
            ModelRelationshipType.Generalization,
            TestModelRelationshipStereotypes.Implementation,
            EntityRelationDirection.Outgoing);

        public static readonly RelatedEntitySpecification ImplementerTypes = new RelatedEntitySpecification(
            ModelRelationshipType.Generalization,
            TestModelRelationshipStereotypes.Implementation,
            EntityRelationDirection.Incoming);
    }
}