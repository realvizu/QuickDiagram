using System.Collections.Generic;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Graph
{
    /// <summary>
    /// A view of a diagram that lets modify the layout but not the components of a diagram.
    /// </summary>
    internal interface IArrangeableDiagram
    {
        IEnumerable<DiagramNode> Nodes { get; }
        IEnumerable<DiagramConnector> Connectors { get; }

        void MoveNode(DiagramNode diagramNode, Point2D newCenter);
        void RerouteConnector(DiagramConnector diagramConnector, Route newRoute);
    }
}
