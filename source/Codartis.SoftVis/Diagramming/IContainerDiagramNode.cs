using System;
using System.Collections.Immutable;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A container node is a diagram node that can have child nodes.
    /// </summary>
    [Immutable]
    public interface IContainerDiagramNode : IDiagramNode
    {
        IImmutableList<IDiagramNode> ChildNodes { get; }

        IDiagramNode AddChildNode(IDiagramNode childNode);
        IDiagramNode RemoveChildNode(IDiagramNode childNode);
    }
}
