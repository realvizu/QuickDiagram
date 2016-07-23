using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A view of a diagram that lets modify the layout but not the components of a diagram.
    /// </summary>
    internal interface IArrangeableDiagram : IDiagram
    {
        void MoveNode(IDiagramNode diagramNode, Point2D newCenter);
        void RerouteConnector(IDiagramConnector diagramConnector, Route newRoute);
    }
}
