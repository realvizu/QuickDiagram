using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;

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

        public event EventHandler<RectMove> CenterChanged;

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
                    CenterChanged?.Invoke(this, new RectMove(DiagramNode, oldValue, value));
            }
        }

        public void RefreshVerticalPosition() => Center = new Point2D(_center.X, GetLayer().CenterY);
        public void FloatPrimaryTree() => ExecuteOnPrimaryTree(i => i.IsFloating = true);

        public IEnumerable<LayoutVertex> GetParents() => _graph.GetParents(this);
        public IEnumerable<LayoutVertex> GetChildren() => _graph.GetChildren(this);
        public IEnumerable<LayoutVertex> GetPositionedChildren() => _graph.GetPositionedChildren(this);
        public IEnumerable<LayoutVertex> GetPrimarySiblings() => _graph.GetPrimarySiblings(this);
        public LayoutVertex GetPrimaryParent() => _graph.GetPrimaryParent(this);
        public IEnumerable<LayoutVertex> GetNonPrimaryParents() => _graph.GetNonPrimaryParents(this);
        public void ExecuteOnPrimaryTree(Action<LayoutVertex> action) => _graph.ExecuteOnPrimaryTree(this, action);

        public bool HasPrimarySiblingsInSameLayer()
        {
            return GetPrimarySiblings().Any(i => i.GetLayerIndex() == GetLayerIndex());
        }

        public bool IsPrimarySiblingOf(LayoutVertex layoutVertex)
        {
            return layoutVertex != null && GetPrimaryParent() == layoutVertex.GetPrimaryParent();
        }

        public DiagramLayer GetLayer() => _graph.GetLayer(this);
        public int GetLayerIndex() => _graph.GetLayerIndex(this);
        public LayoutVertex GetPreviousInLayer() => GetLayer().GetPrevious(this);
        public LayoutVertex GetNextInLayer() => GetLayer().GetNext(this);
        public int GetIndexInLayer() => GetLayer().IndexOf(this);
        public IEnumerable<LayoutVertex> GetOtherPositionedVerticesInLayer() => GetLayer().Where(i => i != this && !i.IsFloating);
        public bool IsLayerItemIndexValid => GetLayer().IsItemIndexValid(this);

        public void RearrangeItemInLayer()
        {
            var layer = GetLayer();
            layer.Remove(this);
            layer.Add(this);
        }

        public LayoutVertex GetPreviousSiblingInLayer()
        {
            var previousVertex = GetPreviousInLayer();
            return IsPrimarySiblingOf(previousVertex) ? previousVertex : null;
        }

        public LayoutVertex GetNextSiblingInLayer()
        {
            var nextVertex = GetNextInLayer();
            return IsPrimarySiblingOf(nextVertex) ? nextVertex : null;
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

        public IEnumerable<LayoutEdge> OutEdges() => _graph.OutEdges(this);
        public IEnumerable<LayoutEdge> InEdges() => _graph.InEdges(this);
    }
}
