namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public class ModelRelationshipAddedEvent : ModelItemEventBase
    {
        public IModelRelationship AddedRelationship { get; }

        public ModelRelationshipAddedEvent(IModelRelationship addedRelationship) 
        {
            AddedRelationship = addedRelationship;
        }
    }
}