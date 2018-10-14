using System;

namespace Codartis.SoftVis.Modeling.Events
{
    public class ModelNodeUpdatedEvent:  ModelEventBase
    {
        public IModelNode OldNode { get; }
        public IModelNode NewNode { get; }

        public ModelNodeUpdatedEvent(IModel newModel, IModelNode oldNode, IModelNode newNode) 
            : base(newModel)
        {
            if (oldNode.Id != newNode.Id)
                throw new InvalidOperationException($"The updated model node id {newNode.Id} does not match the old one {oldNode.Id}.");

            OldNode = oldNode;
            NewNode = newNode;
        }
    }
}