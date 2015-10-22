using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A LayoutVertex that represents a DiagramNode. 
    /// </summary>
    internal class DiagramNodeLayoutVertex : LayoutVertexBase
    {
        public DiagramNode DiagramNode { get; }

        public DiagramNodeLayoutVertex(LayoutGraph graph, DiagramNode diagramNode, bool isFloating)
            :base(graph, isFloating)
        {
            DiagramNode = diagramNode;

            if (diagramNode != null)
                Center = diagramNode.Center;
        }

        public override int Priority => DiagramNode.Priority;
        public override double Width => DiagramNode.Width;
        public override double Height => DiagramNode.Height;
        public override Size2D Size => DiagramNode.Size;

        public override int CompareTo(LayoutVertexBase other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return (other is DiagramNodeLayoutVertex)
                ? DiagramNode.CompareTo(((DiagramNodeLayoutVertex) other).DiagramNode)
                : -1;
        }

        public override string ToString()
        {
            return DiagramNode.ToString();
        }
    }
}
