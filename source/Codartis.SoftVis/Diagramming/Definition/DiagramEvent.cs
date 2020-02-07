using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Describes a change in a diagram by presenting the new diagram state and the changes that led to it.
    /// </summary>
    public struct DiagramEvent
    {
        [NotNull] public IDiagram OldDiagram { get; }
        [NotNull] public IDiagram NewDiagram { get; }
        [NotNull] [ItemNotNull] public IEnumerable<DiagramShapeEventBase> ShapeEvents { get; }

        public DiagramEvent(
            [NotNull] IDiagram oldDiagram,
            [NotNull] IDiagram newDiagram,
            [NotNull] [ItemNotNull] IEnumerable<DiagramShapeEventBase> shapeEvents = null)
        {
            OldDiagram = oldDiagram;
            NewDiagram = newDiagram;
            ShapeEvents = shapeEvents ?? Enumerable.Empty<DiagramShapeEventBase>();
        }

        public bool IsEmpty => !ShapeEvents.Any();
    }
}