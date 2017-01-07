using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram node is a vertex in the diagram graph.
    /// It represents a model entity, eg. a box representing a class.
    /// All diagram nodes have a name, a position and a size.
    /// </summary>
    /// <remarks>
    /// Warning: DiagramNode comparison is based on name order, not position or size!
    /// </remarks>
    public interface IDiagramNode : IDiagramShape, IComparable<IDiagramNode>
    {
        IModelEntity ModelEntity { get; }

        string Name { get; }
        string FullName { get; }
        string Description { get; }
        int Priority { get; }
        Point2D TopLeft { get; }
        Point2D Center { get; set; }
        Size2D Size { get; set; }

        void Rename(string name, string fullName, string description);

        event Action<IDiagramNode, Size2D, Size2D> SizeChanged;
        event Action<IDiagramNode, Point2D, Point2D> CenterChanged;
        event Action<IDiagramNode, string, string, string> Renamed;
    }
}
