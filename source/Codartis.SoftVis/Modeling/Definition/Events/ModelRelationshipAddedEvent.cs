namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public class ModelRelationshipAddedEvent : ModelEventBase
    {
        public IModelRelationship AddedRelationship { get; }

        public ModelRelationshipAddedEvent(IModel newModel, IModelRelationship addedRelationship) 
            : base(newModel)
        {
            AddedRelationship = addedRelationship;
        }
    }
}