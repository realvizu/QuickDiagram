using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// An immutable implementation of the IModelNode interface.
    /// </summary>
    [DebuggerDisplay("{DisplayName} ({GetType().Name})")]
    public class ImmutableModelNode : IModelNode
    {
        public ImmutableList<ImmutableModelNode> ImmutableChildNodes { get; }

        public string DisplayName { get; }
        public string FullName { get; }
        public string Description { get; }
        public ModelOrigin Origin { get; }

        public ImmutableModelNode(ImmutableList<ImmutableModelNode> childNodes, string displayName, string fullName, string description, ModelOrigin origin)
        {
            ImmutableChildNodes = childNodes;
            DisplayName = displayName;
            FullName = fullName;
            Description = description;
            Origin = origin;
        }

        public IEnumerable<IModelNode> ChildNodes => ImmutableChildNodes;
        public virtual int Priority => 0;

        public bool HasChildNode(ImmutableModelNode node) => ImmutableChildNodes.Contains(node);

        public ImmutableModelNode AddChildNode(ImmutableModelNode node)
        {
            if (ImmutableChildNodes.Contains(node))
                throw new InvalidOperationException($"Model node {this} already contains child {node}");

            return new ImmutableModelNode(ImmutableChildNodes.Add(node), DisplayName, FullName, Description, Origin);
        }

        public ImmutableModelNode RemoveChildNode(ImmutableModelNode node)
        {
            if (!ImmutableChildNodes.Contains(node))
                throw new InvalidOperationException($"Model node {this} does not contain child {node}");

            return new ImmutableModelNode(ImmutableChildNodes.Remove(node), DisplayName, FullName, Description, Origin);
        }

        public ImmutableModelNode ReplaceChildNode(ImmutableModelNode oldNode, ImmutableModelNode newNode)
        {
            if (!ImmutableChildNodes.Contains(oldNode))
                throw new InvalidOperationException($"Model node {this} does not contain child {oldNode}");

            return new ImmutableModelNode(ImmutableChildNodes.Replace(oldNode, newNode), DisplayName, FullName, Description, Origin);
        }

        public ImmutableModelNode WithName(string displayName, string fullName, string description)
        {
            return new ImmutableModelNode(ImmutableChildNodes, displayName, fullName, description, Origin);
        }

        public override string ToString() => DisplayName;
    }
}
