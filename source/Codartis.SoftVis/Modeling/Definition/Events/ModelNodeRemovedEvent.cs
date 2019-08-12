namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public class ModelNodeRemovedEvent:  ModelEventBase
    {
        public IModelNode RemovedNode { get; }

        public ModelNodeRemovedEvent(IModel newModel, IModelNode removedNode) 
            : base(newModel)
        {
            RemovedNode = removedNode;
        }
    }
}