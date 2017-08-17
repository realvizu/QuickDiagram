using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Publishes events about model changes.
    /// </summary>
    public interface INotifyModelChanged
    {
        event Action<IModelNode, IModel> NodeAdded;
        event Action<IModelNode, IModel> NodeRemoved;
        event Action<IModelNode, IModelNode, IModel> NodeUpdated;
        event Action<IModelRelationship, IModel> RelationshipAdded;
        event Action<IModelRelationship, IModel> RelationshipRemoved;
        event Action<IModel> ModelCleared;
    }
}
