using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal static class TestEntityRelationTypes
    {
        public static readonly EntityRelationType ImplementedInterfaces = new EntityRelationType(
            ModelRelationshipClassifier.Generalization,
            TestModelRelationshipStereotypes.Implementation,
            EntityRelationDirection.Outgoing);

        public static readonly EntityRelationType ImplementerTypes = new EntityRelationType(
            ModelRelationshipClassifier.Generalization,
            TestModelRelationshipStereotypes.Implementation,
            EntityRelationDirection.Incoming);
    }
}