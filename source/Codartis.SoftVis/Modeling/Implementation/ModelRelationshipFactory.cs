using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Modeling.Implementation
{
    public sealed class ModelRelationshipFactory : IModelRelationshipFactory
    {
        public IModelRelationship CreateRelationship(IModelNode source, IModelNode target, ModelRelationshipStereotype stereotype)
        {
            return new ModelRelationship(ModelRelationshipId.Create(), source, target, stereotype);
        }
    }
}