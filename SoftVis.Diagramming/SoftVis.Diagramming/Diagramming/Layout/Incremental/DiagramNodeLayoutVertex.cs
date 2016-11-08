using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A layout vertex that represents a normal diagram node. 
    /// </summary>
    internal class DiagramNodeLayoutVertex : LayoutVertexBase
    {
        public IDiagramNode DiagramNode { get; }

        public DiagramNodeLayoutVertex(IDiagramNode diagramNode)
        {
            if (diagramNode == null)
                throw new ArgumentNullException(nameof(diagramNode));

            DiagramNode = diagramNode;
        }

        public override bool IsDummy => false;
        public override string Name => DiagramNode.Name;
        public override int Priority => DiagramNode.Priority;
        public override Size2D Size => DiagramNode.Size;
    }
}
