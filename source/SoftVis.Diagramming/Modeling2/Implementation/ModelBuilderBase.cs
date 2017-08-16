using System;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// Abstract base class for model builders.
    /// Responsible for adding, removing, updating nodes and relationships in a model.
    /// </summary>
    public abstract class ModelBuilderBase : IModelBuilder
    {
        private readonly IImmutableModelFactory _modelFactory;
        private ImmutableModel _currentModel;

        public event Action<IModelNode, IModel> NodeAdded;
        public event Action<IModelNode, IModel> NodeRemoved;
        public event Action<IModelNode, IModelNode, IModel> NodeUpdated;
        public event Action<IModelRelationship, IModel> RelationshipAdded;
        public event Action<IModelRelationship, IModel> RelationshipRemoved;
        public event Action<IModel> ModelCleared;

        protected ModelBuilderBase(IImmutableModelFactory modelFactory)
        {
            _modelFactory = modelFactory ?? throw new ArgumentNullException(nameof(modelFactory));
            _currentModel = _modelFactory.CreateModel();
        }

        public IModel CurrentModel => _currentModel;

        public IModel ClearModel()
        {
            _currentModel = _modelFactory.CreateModel();
            ModelCleared?.Invoke(_currentModel);
            return _currentModel;
        }

        protected IModel AddNode(ImmutableModelNodeBase node, ImmutableModelNodeBase parentNode = null)
        {
            _currentModel = _currentModel.AddNode(node, parentNode);
            NodeAdded?.Invoke(node, _currentModel);
            return _currentModel;
        }

        protected IModel RemoveNode(ImmutableModelNodeBase node)
        {
            _currentModel = _currentModel.RemoveNode(node);
            NodeRemoved?.Invoke(node, _currentModel);
            return _currentModel;
        }

        protected IModel UpdateNode(ImmutableModelNodeBase oldNode, ImmutableModelNodeBase newNode)
        {
            _currentModel = _currentModel.UpdateNode(oldNode, newNode);
            NodeUpdated?.Invoke(oldNode, newNode, _currentModel);
            return _currentModel;
        }

        protected IModel AddRelationship(ModelRelationshipBase relationship)
        {
            _currentModel = _currentModel.AddRelationship(relationship);
            RelationshipAdded?.Invoke(relationship, _currentModel);
            return _currentModel;
        }

        protected IModel RemoveRelationship(ModelRelationshipBase relationship)
        {
            _currentModel = _currentModel.RemoveRelationship(relationship);
            RelationshipRemoved?.Invoke(relationship, _currentModel);
            return _currentModel;
        }
    }
}
