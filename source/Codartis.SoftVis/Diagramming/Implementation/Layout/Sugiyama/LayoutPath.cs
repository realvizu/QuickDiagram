using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Graphs;
using Codartis.Util;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama
{
    /// <summary>
    /// A list of proper layout edges that form a path.
    /// </summary>
    /// <remarks>
    /// Invariants:
    /// <para>Source and Target are always of type DiagramNodeLayoutVertex.</para>
    /// <para>Interim vertices are always of type DummyLayoutVertex.</para>
    /// </remarks>
    internal sealed class LayoutPath : Path<LayoutVertexBase, GeneralLayoutEdge>, IEdge<DiagramNodeLayoutVertex>
    {
        public LayoutPath(DiagramNodeLayoutVertex sourceVertex, DiagramNodeLayoutVertex targetVertex,
            IDiagramConnector diagramConnector)
            : this(new GeneralLayoutEdge(sourceVertex, targetVertex, diagramConnector))
        {
        }

        public LayoutPath(GeneralLayoutEdge generalLayoutEdge)
            : this(generalLayoutEdge.ToEnumerable())
        {
        }

        public LayoutPath(IEnumerable<GeneralLayoutEdge> edges)
            : base(edges)
        {
            CheckPrivateInvariants();
        }

        DiagramNodeLayoutVertex IEdge<DiagramNodeLayoutVertex>.Source => Source as DiagramNodeLayoutVertex;
        DiagramNodeLayoutVertex IEdge<DiagramNodeLayoutVertex>.Target => Target as DiagramNodeLayoutVertex;
        public DiagramNodeLayoutVertex PathSource => ((IEdge<DiagramNodeLayoutVertex>)this).Source;
        public DiagramNodeLayoutVertex PathTarget => ((IEdge<DiagramNodeLayoutVertex>)this).Target;

        private IEnumerable<LayoutVertexBase> InterimVerticesPrivate => this.Skip(1).Select(i => i.Source);
        public IEnumerable<DummyLayoutVertex> InterimVertices => InterimVerticesPrivate.OfType<DummyLayoutVertex>();
        public IDiagramConnector DiagramConnector => Edges.FirstOrDefault()?.DiagramConnector;

        public void Substitute(int atIndex, int removeEdgeCount, params GeneralLayoutEdge[] newEdges)
        {
            for (var i = 0; i < removeEdgeCount; i++)
                Edges.RemoveAt(atIndex);

            for (var i = 0; i < newEdges.Length; i++)
                Edges.Insert(atIndex + i, newEdges[i]);

            CheckAllInvariants();
        }

        private void CheckAllInvariants()
        {
            CheckInvariant();
            CheckPrivateInvariants();
        }

        private void CheckPrivateInvariants()
        {
            if (Length > 0 && !(Source is DiagramNodeLayoutVertex))
                throw new LayoutPathException("Source must be DiagramNodeLayoutVertex");

            if (Length > 0 && !(Target is DiagramNodeLayoutVertex))
                throw new LayoutPathException("Target must be DiagramNodeLayoutVertex");

            if (InterimVerticesPrivate.Any(i => !(i is DummyLayoutVertex)))
                throw new LayoutPathException("All interim vertices must be DummyLayoutVertex.");

            if (Edges.GroupBy(i => i.DiagramConnector).Count() != 1)
                throw new LayoutPathException("All edges must reference the same diagram connector.");
        }
    }
}
