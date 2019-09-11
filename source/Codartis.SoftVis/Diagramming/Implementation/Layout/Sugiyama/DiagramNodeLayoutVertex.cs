using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama
{
    /// <summary>
    /// A layout vertex that represents a normal diagram node. 
    /// </summary>
    internal class DiagramNodeLayoutVertex : LayoutVertexBase
    {
        [NotNull] public IDiagramNode DiagramNode { get; }
        public override int Priority { get; }
        private Size2D _size;

        public DiagramNodeLayoutVertex([NotNull] IDiagramNode diagramNode, int priority)
        {
            DiagramNode = diagramNode;
            _size = diagramNode.Size;
            Priority = priority;
        }

        public override bool IsDummy => false;
        public override string Name => DiagramNode.Name;
        public override Size2D Size => _size;

        public void Resize(Size2D newSize) => _size = newSize;
    }
}