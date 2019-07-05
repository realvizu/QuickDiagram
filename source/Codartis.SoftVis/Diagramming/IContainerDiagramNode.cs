using System.Collections.Generic;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A container node is a diagram node that can have child nodes.
    /// </summary>
    public interface IContainerDiagramNode : IDiagramNode
    {
        IEnumerable<IDiagramNode> ChildNodes { get; }
        Rect2D EmbeddedDiagramRect { get; }

        IDiagramNode WithChildNode(IDiagramNode childNode);
        IDiagramNode WithoutChildNode(IDiagramNode childNode);
    }
}
