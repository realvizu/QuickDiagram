using System;
using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama
{
    /// <summary>
    /// A layout vertex that represents a normal diagram node. 
    /// </summary>
    internal class DiagramNodeLayoutVertex : LayoutVertexBase
    {
        [NotNull] public IDiagramNode DiagramNode { get; }
        public override int Priority { get; }

        public DiagramNodeLayoutVertex([NotNull] IDiagramNode diagramNode, int priority)
        {
            DiagramNode = diagramNode;
            Priority = priority;
        }

        public override bool IsDummy => false;
        public override string Name => DiagramNode.Name;
        public override Size2D Size => DiagramNode.Size;

        public void Resize(Size2D size) => throw new NotSupportedException();
    }
}