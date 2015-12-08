using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal static class TestRelationshipSpecifications
    {
        public static readonly RelationshipSpecification ImplementedInterfaces =
            new RelationshipSpecification(ModelRelationshipDirection.Outgoing, ModelRelationshipType.Generalization,
                TestModelRelationshipStereotype.Implementation);

        public static readonly RelationshipSpecification ImplementerTypes =
            new RelationshipSpecification(ModelRelationshipDirection.Incoming, ModelRelationshipType.Generalization,
                TestModelRelationshipStereotype.Implementation);
    }
}