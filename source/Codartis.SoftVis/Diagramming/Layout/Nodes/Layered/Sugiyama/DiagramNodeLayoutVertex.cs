using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama
{
    /// <summary>
    /// A layout vertex that represents a normal diagram node. 
    /// </summary>
    internal class DiagramNodeLayoutVertex : LayoutVertexBase
    {
        private Size2D _size;
        public IDiagramNode DiagramNode { get; }
        public override string Name { get; }
        public override int Priority { get; }

        public DiagramNodeLayoutVertex(IDiagramNode diagramNode, string name, int priority)
        {
            DiagramNode = diagramNode;
            Name = name;
            Priority = priority;

            _size = Size2D.Zero;
        }

        public override bool IsDummy => false;
        public override Size2D Size => _size;

        public void Resize(Size2D size) => _size = size;
    }
}
