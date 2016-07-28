using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    public static class ModelRelationshipExtensions
    {
        public static bool IsGeneralization(this IModelRelationship relationship)
        {
            return relationship.Type == ModelRelationshipType.Generalization
                   && relationship.Stereotype == ModelRelationshipStereotype.None;
        }

        public static bool IsInterfaceImplementation(this IModelRelationship relationship)
        {
            return relationship.Type == ModelRelationshipType.Generalization
                   && relationship.Stereotype == ModelRelationshipStereotypes.Implementation;
        }
    }
}
