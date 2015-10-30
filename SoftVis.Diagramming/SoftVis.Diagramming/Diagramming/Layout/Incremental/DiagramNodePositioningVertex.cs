using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A positioning vertex that represents a normal diagram node. 
    /// </summary>
    internal class DiagramNodePositioningVertex : PositioningVertexBase
    {
        public DiagramNode DiagramNode { get; }

        public DiagramNodePositioningVertex(PositioningGraph graph, DiagramNode diagramNode, bool isFloating = true)
            :base(graph, isFloating)
        {
            if (diagramNode == null)
                throw new ArgumentNullException(nameof(diagramNode));

            DiagramNode = diagramNode;
        }

        public override string Name => DiagramNode.Name;
        public override int Priority => DiagramNode.Priority;
        public override double Width => DiagramNode.Width;
        public override double Height => DiagramNode.Height;
        public override Size2D Size => DiagramNode.Size;

        public override int CompareTo(PositioningVertexBase other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return (other is DiagramNodePositioningVertex)
                ? DiagramNode.CompareTo(((DiagramNodePositioningVertex) other).DiagramNode)
                : -1;
        }

        public override string ToString()
        {
            return DiagramNode.ToString();
        }
    }
}
