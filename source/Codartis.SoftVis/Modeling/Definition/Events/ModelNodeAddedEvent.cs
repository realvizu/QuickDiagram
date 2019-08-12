namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public class ModelNodeAddedEvent:  ModelEventBase
    {
        public IModelNode AddedNode { get; }

        public ModelNodeAddedEvent(IModel newModel, IModelNode addedNode) 
            : base(newModel)
        {
            AddedNode = addedNode;
        }
    }
}