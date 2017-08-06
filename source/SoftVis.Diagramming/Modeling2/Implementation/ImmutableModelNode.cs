using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// Abstract base class for immutable implementations of the IModelNode interface.
    /// </summary>
    /// <remarks>
    /// Descendants must keep the immutability: mutators must return a new (mutated) instance.
    /// The Id field is used to track identity through mutated instances so its value must be kept unchanged by all mutators.
    /// </remarks>
    [DebuggerDisplay("{GetType().Name} {DisplayName} [{Id}]")]
    public abstract class ImmutableModelNode : IModelNode
    {
        public ModelItemId Id { get; }
        public string DisplayName { get; }
        public string FullName { get; }
        public string Description { get; }
        public ModelOrigin Origin { get; }
        public ImmutableList<ImmutableModelNode> ImmutableChildNodes { get; }

        protected ImmutableModelNode(ModelItemId id, string displayName, string fullName, string description, 
            ModelOrigin origin, ImmutableList<ImmutableModelNode> childNodes)
        {
            Id = id;
            DisplayName = displayName;
            FullName = fullName;
            Description = description;
            Origin = origin;
            ImmutableChildNodes = childNodes;
        }

        public IEnumerable<IModelNode> ChildNodes => ImmutableChildNodes;
        public virtual int Priority => 0;

        public bool HasChildNode(ImmutableModelNode node) => ImmutableChildNodes.Contains(node);

        public ImmutableModelNode AddChildNode(ImmutableModelNode node)
        {
            if (ImmutableChildNodes.Contains(node))
                throw new InvalidOperationException($"Model node {this} already contains child {node}");

            return CreateInstance(Id, DisplayName, FullName, Description, Origin, ImmutableChildNodes.Add(node));
        }

        public ImmutableModelNode RemoveChildNode(ImmutableModelNode node)
        {
            if (!ImmutableChildNodes.Contains(node))
                throw new InvalidOperationException($"Model node {this} does not contain child {node}");

            return CreateInstance(Id, DisplayName, FullName, Description, Origin, ImmutableChildNodes.Remove(node));
        }

        public ImmutableModelNode ReplaceChildNode(ImmutableModelNode oldNode, ImmutableModelNode newNode)
        {
            if (!ImmutableChildNodes.Contains(oldNode))
                throw new InvalidOperationException($"Model node {this} does not contain child {oldNode}");

            return CreateInstance(Id, DisplayName, FullName, Description, Origin, ImmutableChildNodes.Replace(oldNode, newNode));
        }

        public ImmutableModelNode WithName(string displayName, string fullName, string description)
        {
            return CreateInstance(Id, displayName, fullName, description, Origin, ImmutableChildNodes);
        }

        protected abstract ImmutableModelNode CreateInstance(ModelItemId id,
            string displayName, string fullName, string description,
            ModelOrigin origin, ImmutableList<ImmutableModelNode> childNodes);

        protected bool Equals(ImmutableModelNode other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ImmutableModelNode) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ImmutableModelNode left, ImmutableModelNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ImmutableModelNode left, ImmutableModelNode right)
        {
            return !Equals(left, right);
        }

        public override string ToString() => $"{GetType().Name} {DisplayName} [{Id}]";
    }
}
