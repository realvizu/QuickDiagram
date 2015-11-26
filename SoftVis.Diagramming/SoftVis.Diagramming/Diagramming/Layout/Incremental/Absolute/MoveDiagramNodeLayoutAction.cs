using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute
{
    /// <summary>
    /// A layout action that moves a DiagramNodeLayoutVertex.
    /// </summary>
    internal class MoveDiagramNodeLayoutAction : IMoveDiagramNodeLayoutAction
    {
        private DiagramNodeLayoutVertex Vertex { get; }
        public Point2D From { get; }
        public Point2D To { get; }
        public Point2D By { get; }

        public MoveDiagramNodeLayoutAction(DiagramNodeLayoutVertex diagramNodeLayoutVertex, Point2D @from, Point2D to)
        {
            Vertex = diagramNodeLayoutVertex;
            From = @from;
            To = to;
            By = To - From;
        }

        public DiagramNode DiagramNode => Vertex.DiagramNode;

        public void AcceptVisitor(ILayoutActionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}