using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Incremental
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
            DiagramConnector diagramConnector)
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
        public DiagramConnector DiagramConnector => Edges.FirstOrDefault()?.DiagramConnector;

        public void Substitute(int atIndex, int removeEdgeCount, params GeneralLayoutEdge[] newEdges)
        {
            for (var i = 0; i < removeEdgeCount; i++)
                Edges.RemoveAt(atIndex);

            for (var i = 0; i < newEdges.Length; i++)
                Edges.Insert(atIndex + i, newEdges[i]);

            CheckAllInvariants();
        }

        public Route GetRoute(LayoutVertexToPointMap vertexCenters)
        {
            var sourceRect = vertexCenters.GetRect(Source);
            var interimRoutePoints = InterimVertices.Select(vertexCenters.Get).ToArray();
            var targetRect = vertexCenters.GetRect(Target);

            var secondPoint = interimRoutePoints.Any()
                ? interimRoutePoints.First()
                : targetRect.Center;
            var penultimatePoint = interimRoutePoints.Any()
                ? interimRoutePoints.Last()
                : sourceRect.Center;

            return new Route
            {
                sourceRect.GetAttachPointToward(secondPoint),
                interimRoutePoints,
                targetRect.GetAttachPointToward(penultimatePoint)
            };
        }

        private void CheckAllInvariants()
        {
            base.CheckInvariant();
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
