using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Abstract base class for vertices used in a LayoutGraph. 
    /// </summary>
    /// <remarks>
    /// <para>LayoutVertices can float which indicates that the edge does not take part in layout calculations.</para>
    /// <para>LayoutVertices can be compared to each other.</para>
    /// </remarks>
    internal abstract class LayoutVertexBase : IRect, IComparable<LayoutVertexBase>
    {
        private readonly LayoutGraph _graph;
        private Point2D _center;
        public bool IsFloating { get; set; }

        public event EventHandler<RectMove> CenterChanged;

        protected LayoutVertexBase(LayoutGraph graph, bool isFloating)
        {
            _graph = graph;
            IsFloating = isFloating;
        }

        public abstract int Priority { get; }
        public abstract double Width { get; }
        public abstract double Height { get; }
        public abstract Size2D Size { get; }
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

                CenterChanged?.Invoke(this, new RectMove(Rect, oldValue, value));
            }
        }

        public void RefreshVerticalPosition() => Center = new Point2D(_center.X, GetLayer().CenterY);
        public void FloatPrimaryTree() => ExecuteOnPrimaryDescendantVertices(i => i.IsFloating = true);
        public IEnumerable<LayoutEdge> InEdges => _graph.InEdges(this);
        public IEnumerable<LayoutPath> InPaths => InEdges.Select(LayoutPath.GetFromInEdge);
        public IEnumerable<LayoutEdge> OutEdges => _graph.OutEdges(this);
        public IEnumerable<LayoutPath> OutPaths => OutEdges.Select(LayoutPath.GetFromOutEdge);
        public IEnumerable<LayoutVertexBase> GetParents() => _graph.GetParents(this);
        public LayoutVertexBase GetPrimaryParent() => _graph.GetPrimaryParent(this);
        public IEnumerable<LayoutVertexBase> GetNonPrimaryParents() => _graph.GetNonPrimaryParents(this);
        public IEnumerable<LayoutVertexBase> GetChildren() => _graph.GetChildren(this);
        public IEnumerable<LayoutVertexBase> GetPrimaryChildren() => _graph.GetPrimaryChildren(this);
        public IEnumerable<LayoutVertexBase> GetPrimaryPositionedChildren() => _graph.GetPrimaryPositionedChildren(this);
        public IEnumerable<LayoutVertexBase> GetPrimarySiblings() => _graph.GetPrimarySiblings(this);
        public void ExecuteOnPrimaryDescendantVertices(Action<LayoutVertexBase> action) => _graph.ExecuteOnPrimaryDescendantVertices(this, action);

        public bool HasPrimarySiblingsInSameLayer()
        {
            return GetPrimarySiblings().Any(i => i.LayerIndex == LayerIndex);
        }

        public bool IsPrimarySiblingOf(LayoutVertexBase layoutVertex)
        {
            return layoutVertex != null && GetPrimaryParent() == layoutVertex.GetPrimaryParent();
        }

        public LayoutVertexLayer GetLayer() => _graph.GetLayer(this);
        public int LayerIndex => _graph.GetLayerIndex(this);
        public LayoutVertexBase GetPreviousInLayer() => GetLayer().GetPrevious(this);
        public LayoutVertexBase GetNextInLayer() => GetLayer().GetNext(this);
        public int GetIndexInLayer() => GetLayer().IndexOf(this);
        public IEnumerable<LayoutVertexBase> GetOtherPositionedVerticesInLayer() => GetLayer().Where(i => i != this && !i.IsFloating);
        public bool IsLayerItemIndexValid => GetLayer().IsItemIndexValid(this);
        public bool IsLayerIndexValid => LayerIndex > OutEdges.Select(i => i.Source.LayerIndex).Max();

        public void RearrangeItemInLayer()
        {
            var layer = GetLayer();
            layer.Remove(this);
            layer.Add(this);
        }

        public LayoutVertexBase GetPreviousSiblingInLayer()
        {
            var previousVertex = GetPreviousInLayer();
            return IsPrimarySiblingOf(previousVertex) ? previousVertex : null;
        }

        public LayoutVertexBase GetNextSiblingInLayer()
        {
            var nextVertex = GetNextInLayer();
            return IsPrimarySiblingOf(nextVertex) ? nextVertex : null;
        }

        public bool OverlapsWith(LayoutVertexBase otherVertex, double marginX)
        {
            return Rect.WithMarginX(marginX).Intersect(otherVertex.Rect) != Rect2D.Empty;
        }

        public abstract int CompareTo(LayoutVertexBase other);

        public bool Precedes(LayoutVertexBase otherVertex) => otherVertex != null && CompareTo(otherVertex) < 0;

    }
}
