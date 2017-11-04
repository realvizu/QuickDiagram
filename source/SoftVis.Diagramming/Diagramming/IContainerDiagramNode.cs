using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A container node is a diagram node that can have child nodes.
    /// </summary>
    public interface IContainerDiagramNode : IDiagramNode
    {
        IEnumerable<IDiagramNode> ChildNodes { get; }

        IDiagramNode AddChildNode(IDiagramNode childNode);
        IDiagramNode RemoveChildNode(IDiagramNode childNode);
    }
}
