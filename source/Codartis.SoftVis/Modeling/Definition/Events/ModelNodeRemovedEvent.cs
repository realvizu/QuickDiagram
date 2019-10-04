namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public class ModelNodeRemovedEvent : ModelItemEventBase
    {
        public IModelNode RemovedNode { get; }

        public ModelNodeRemovedEvent(IModelNode removedNode)
        {
            RemovedNode = removedNode;
        }
    }
}