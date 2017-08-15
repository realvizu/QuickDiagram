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
        public string Name { get; }
        public ModelOrigin Origin { get; }

        protected ImmutableModelNodeBase(ModelItemId id, string name, ModelOrigin origin)
        {
            Id = id;
            Name = name;
            Origin = origin;
        }

        public virtual int Priority => 0;

        public ImmutableModelNodeBase WithName(string name) => CreateInstance(Id, name, Origin);

        protected abstract ImmutableModelNodeBase CreateInstance(ModelItemId id, string name, ModelOrigin origin);

        public override string ToString() => $"{GetType().Name} {Name} [{Id}]";
    }
}
