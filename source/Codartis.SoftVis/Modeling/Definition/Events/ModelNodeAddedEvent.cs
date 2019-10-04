namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public class ModelNodeAddedEvent:  ModelItemEventBase
    {
        public IModelNode AddedNode { get; }

        public ModelNodeAddedEvent(IModelNode addedNode) 
        {
            AddedNode = addedNode;
        }
    }
}