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

        public DiagramNodePositioningVertex(DiagramNode diagramNode, bool isFloating = true)
            :base(isFloating)
        {
            if (diagramNode == null)
                throw new ArgumentNullException(nameof(diagramNode));

            DiagramNode = diagramNode;
        }

        public override bool IsDummy => false;
        public override string Name => DiagramNode.Name;
        public override int Priority => DiagramNode.Priority;
        public override Size2D Size => DiagramNode.Size;

        public override string ToString()
        {
            return DiagramNode.ToString();
        }
    }
}
