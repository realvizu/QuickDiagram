using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Abstract base class for vertices used in a positioning graph. 
    /// </summary>
    /// <remarks>
    /// <para>LayoutVertices can float which indicates that the edge does not take part in layout calculations.</para>
    /// <para>LayoutVertices can be compared to each other.</para>
    /// </remarks>
    internal abstract class PositioningVertexBase : IRect, IComparable<PositioningVertexBase>
    {
        private readonly PositioningGraph _graph;
        public bool IsFloating { get; set; }
        public Point2D Center { get; set; }

        protected PositioningVertexBase(PositioningGraph graph, bool isFloating)
        {
            _graph = graph;
            IsFloating = isFloating;
            Center = Point2D.Empty;
        }

        public abstract string Name { get; }
        public abstract int Priority { get; }
        public abstract double Width { get; }
        public abstract double Height { get; }
        public Size2D Size => new Size2D(Width, Height);
        public Rect2D Rect => new Rect2D(Center.X - Width / 2, Center.Y - Height / 2, Center.X + Width / 2, Center.Y + Height / 2);
        public double Left => Rect.Left;
        public double Right => Rect.Right;
        public double Top => Rect.Top;
        public double Bottom => Rect.Bottom;

        public virtual string NameForComparison => Name;
        public int CompareTo(PositioningVertexBase other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return string.Compare(NameForComparison, other.NameForComparison, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Precedes(PositioningVertexBase otherVertex) => otherVertex != null && CompareTo(otherVertex) < 0;

        public void FloatPrimaryTree() => ExecuteOnPrimaryDescendantVertices(i => i.IsFloating = true);

        public IEnumerable<PositioningEdge> InEdges => _graph.InEdges(this);
        public IEnumerable<PositioningEdge> OutEdges => _graph.OutEdges(this);
        public IEnumerable<PositioningEdge> AllEdges => _graph.GetAllEdges(this);

        public IEnumerable<PositioningVertexBase> GetParents() => _graph.GetParents(this);
        public PositioningVertexBase GetPrimaryParent() => _graph.GetPrimaryParent(this);
        public IEnumerable<PositioningVertexBase> GetNonPrimaryParents() => _graph.GetNonPrimaryParents(this);
        public IEnumerable<PositioningVertexBase> GetChildren() => _graph.GetChildren(this);
        public IEnumerable<PositioningVertexBase> GetPrimaryChildren() => _graph.GetPrimaryChildren(this);
        public IEnumerable<PositioningVertexBase> GetPrimaryPositionedChildren() => _graph.GetPrimaryPositionedChildren(this);
        public IEnumerable<PositioningVertexBase> GetPrimarySiblings() => _graph.GetPrimarySiblings(this);
        public IEnumerable<PositioningVertexBase> GetPlacedPrimarySiblings() => GetPrimarySiblings().Where(i => !i.IsFloating);
        public void ExecuteOnPrimaryDescendantVertices(Action<PositioningVertexBase> action) => _graph.ExecuteOnPrimaryDescendantVertices(this, action);

        public IEnumerable<PositioningVertexBase> GetPrimarySiblingsInSameLayer()
        {
            var layerIndex = GetLayerIndex();
            return GetPrimarySiblings().Where(i => i.GetLayerIndex() == layerIndex);
        }

        public bool HasPlacedPrimarySiblingsInSameLayer()
        {
            return GetPrimarySiblingsInSameLayer().Any(i => !i.IsFloating);
        }

        public bool IsPlacedPrimarySiblingOf(PositioningVertexBase layoutVertex)
        {
            return layoutVertex != null && !layoutVertex.IsFloating && GetPrimaryParent() == layoutVertex.GetPrimaryParent();
        }

        public PositioningVertexLayer GetLayer() => _graph.GetLayer(this);
        public int GetLayerIndex() => _graph.GetLayerIndex(this);
        public virtual int GetNonDummyLayerIndex() => GetLayerIndex();
        public PositioningVertexBase GetPreviousInLayer() => GetLayer().GetPrevious(this);
        public PositioningVertexBase GetNextInLayer() => GetLayer().GetNext(this);
        public int GetIndexInLayer() => GetLayer().IndexOf(this);
        public IEnumerable<PositioningVertexBase> GetOtherPositionedVerticesInLayer() => GetLayer().Where(i => i != this && !i.IsFloating);
        public bool IsLayerItemIndexValid() => GetLayer().IsItemIndexValid(this);
        public bool IsLayerIndexValid() => GetLayerIndex() > OutEdges.Select(i => i.Source.GetLayerIndex()).Max();

        public PositioningVertexBase GetPreviousPlacedSiblingInLayer()
        {
            var previousVertex = GetPreviousInLayer();
            return IsPlacedPrimarySiblingOf(previousVertex) ? previousVertex : null;
        }

        public PositioningVertexBase GetNextPlacedSiblingInLayer()
        {
            var nextVertex = GetNextInLayer();
            return IsPlacedPrimarySiblingOf(nextVertex) ? nextVertex : null;
        }
    }
}
