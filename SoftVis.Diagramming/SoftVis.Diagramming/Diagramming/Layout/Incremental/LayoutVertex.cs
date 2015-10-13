using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A vertex used in a LayoutGraph. 
    /// Either a dummy vertex or encapulates a real vertex.
    /// </summary>
    /// <remarks>
    /// <para>LayoutVertices can float which indicates that the edge 
    /// does not take part in layout calculations.</para>
    /// <para>Dummy vertices are used to break long edges spanning more than two layers.
    /// They ensure that adjacent vertices in the graph are always on adjacent layers.</para>
    /// <para>LayoutVertices can be compared to each other.
    /// Comparing to a dummy yields 0 (equal).
    /// Comparing real vertices yields the comparison of thier originals.</para>
    /// </remarks>
    internal class LayoutVertex : IRect, IComparable<LayoutVertex>
    {
        private readonly LayoutGraph _graph;
        private Point2D _center;
        public DiagramNode DiagramNode { get; }
        public bool IsFloating { get; set; }

        public event EventHandler<RectMoveEventArgs> CenterChanged;

        public LayoutVertex(LayoutGraph graph, DiagramNode diagramNode, bool isFloating)
        {
            _graph = graph;
            DiagramNode = diagramNode;
            IsFloating = isFloating;

            if (diagramNode != null)
                _center = diagramNode.Center;
        }

        public bool IsDummy => DiagramNode == null;
        public double Width => DiagramNode?.Width ?? 0d;
        public double Height => DiagramNode?.Height ?? 0d;
        public Size2D Size => DiagramNode?.Size ?? Size2D.Empty;
        public Rect2D Rect => new Rect2D(Center.X - Width / 2, Center.Y - Height / 2, Center.X + Width / 2, Center.Y + Height / 2);
        public double Left => Rect.Left;
        public double Right => Rect.Right;
        public double Top => Rect.Top;
        public double Bottom => Rect.Bottom;

        public Point2D Center
        {
            get { return _center; }
            set
            {
                IsFloating = false;

                if (_center == value)
                    return;

                var oldValue = _center;
                _center = value;

                if (!IsDummy)
                    CenterChanged?.Invoke(this, new RectMoveEventArgs(DiagramNode, oldValue, value));
            }
        }

        public IEnumerable<LayoutVertex> GetParents() => _graph.GetOutNeighbours(this);
        public IEnumerable<LayoutVertex> GetChildren() => _graph.GetInNeighbours(this);
        public IEnumerable<LayoutVertex> GetPositionedChildren() => GetChildren().Where(i => !i.IsFloating);
        public IEnumerable<LayoutVertex> GetSiblings() => GetParents().SelectMany(i => i.GetChildren()).Where(i => i != this);
        public LayoutVertex GetParent() => GetParents().OrderByDescending(i => i.DiagramNode?.Priority).FirstOrDefault();
        public bool HasSiblings() => GetSiblings().Any();

        public bool IsSiblingOf(LayoutVertex layoutVertex)
        {
            return layoutVertex != null && GetParents().Intersect(layoutVertex.GetParents()).Any();
        }

        public DiagramLayer GetLayer() => _graph.GetLayer(this);
        public int GetLayerIndex() => _graph.GetLayerIndex(this);
        public LayoutVertex GetPreviousInLayer() => GetLayer().GetPrevious(this);
        public LayoutVertex GetNextInLayer() => GetLayer().GetNext(this);
        public IEnumerable<LayoutVertex> GetOtherPositionedVerticesInLayer() => GetLayer().Where(i => i != this && !i.IsFloating);

        public LayoutVertex GetPreviousSibling()
        {
            var previousVertex = GetPreviousInLayer();
            return IsSiblingOf(previousVertex) ? previousVertex : null;
        }

        public LayoutVertex GetNextSibling()
        {
            var nextVertex = GetNextInLayer();
            return IsSiblingOf(nextVertex) ? nextVertex : null;
        }

        public bool OverlapsWith(LayoutVertex otherVertex, double marginX)
        {
            return Rect.WithMarginX(marginX).Intersect(otherVertex.Rect) != Rect2D.Empty;
        }

        public int CompareTo(LayoutVertex other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (IsDummy || other.IsDummy)
                return 0;

            return DiagramNode.CompareTo(other.DiagramNode);
        }

        public bool Precedes(LayoutVertex otherVertex) => otherVertex != null && CompareTo(otherVertex) < 0;

        public override string ToString()
        {
            return DiagramNode?.ToString() ?? "(dummy)";
        }
    }
}
