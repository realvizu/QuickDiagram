using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama
{
    /// <summary>
    /// A layout vertex that represents a normal diagram node. 
    /// </summary>
    internal class DiagramNodeLayoutVertex : LayoutVertexBase
    {
        public IDiagramNode DiagramNode { get; }
        public override string Name { get; }
        public override int Priority { get; }

        public DiagramNodeLayoutVertex(IDiagramNode diagramNode, string name, int priority)
        {
            DiagramNode = diagramNode;
            Name = name;
            Priority = priority;
        }

        public override bool IsDummy => false;
        public override Size2D Size => DiagramNode.Size;

        public void Resize(Size2D size) => throw new NotSupportedException();
    }
}
