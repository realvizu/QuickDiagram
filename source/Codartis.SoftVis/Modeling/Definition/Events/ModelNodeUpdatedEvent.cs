namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public sealed class ModelNodeUpdatedEvent: ModelItemEventBase
    {
        public IModelNode OldNode { get; }
        public IModelNode NewNode { get; }

        public ModelNodeUpdatedEvent(IModelNode oldNode, IModelNode newNode)
        {
            OldNode = oldNode;
            NewNode = newNode;
        }
    }
}
