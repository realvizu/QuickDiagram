using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal static class TestEntityRelationTypes
    {
        public static readonly EntityRelationType ImplementedInterfaces = new EntityRelationType("Implemented interfaces",
            ModelRelationshipClassifier.Generalization, TestModelRelationshipStereotypes.Implementation, EntityRelationDirection.Outgoing);

        public static readonly EntityRelationType ImplementerTypes = new EntityRelationType("Implementing types",
            ModelRelationshipClassifier.Generalization, TestModelRelationshipStereotypes.Implementation, EntityRelationDirection.Incoming);
    }
}