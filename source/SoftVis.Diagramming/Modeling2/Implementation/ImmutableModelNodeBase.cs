namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// Abstract base class for immutable implementations of the IModelNode interface.
    /// </summary>
    /// <remarks>
    /// Descendants must keep the immutability: mutators must return a new (mutated) instance.
    /// The Id field is used to track identity through mutated instances so its value must be kept unchanged by all mutators.
    /// </remarks>
    public abstract class ImmutableModelNodeBase : IModelNode
    {
        public ModelItemId Id { get; }
        public string DisplayName { get; }
        public string FullName { get; }
        public string Description { get; }
        public ModelOrigin Origin { get; }

        protected ImmutableModelNodeBase(ModelItemId id, string displayName, string fullName, string description, ModelOrigin origin)
        {
            Id = id;
            DisplayName = displayName;
            FullName = fullName;
            Description = description;
            Origin = origin;
        }

        public virtual int Priority => 0;

        public ImmutableModelNodeBase WithName(string displayName, string fullName, string description)
        {
            return CreateInstance(Id, displayName, fullName, description, Origin);
        }

        protected abstract ImmutableModelNodeBase CreateInstance(ModelItemId id, string displayName, string fullName, string description, ModelOrigin origin);

        public override string ToString() => $"{GetType().Name} {DisplayName} [{Id}]";
    }
}
