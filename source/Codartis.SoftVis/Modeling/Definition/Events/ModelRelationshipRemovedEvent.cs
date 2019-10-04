namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public class ModelRelationshipRemovedEvent: ModelItemEventBase
    {
        public IModelRelationship RemovedRelationship { get; }

        public ModelRelationshipRemovedEvent(IModelRelationship removedRelationship) 
        {
            RemovedRelationship = removedRelationship;
        }
    }
}