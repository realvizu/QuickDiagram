using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram node is a vertex in the diagram graph that represents a model node (eg. a box representing a class).
    /// All diagram nodes have a name, a size and a position.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// The name plays role in layout as a basis for ordering.
    /// </remarks>
    public interface IDiagramNode : IUpdatableImmutableVertex<ModelNodeId>, IDiagramShape, IComparable<IDiagramNode>
    {
        IModelNode ModelNode { get; }

        string Name { get; }
        Size2D Size { get; }
        Point2D Center { get; }
        Point2D TopLeft { get; }
        IContainerDiagramNode ParentDiagramNode { get; }
        bool HasParent { get; }

        IDiagramNode WithModelNode(IModelNode modelNode);
        IDiagramNode WithSize(Size2D newSize);
        IDiagramNode WithCenter(Point2D newCenter);
    }
}
