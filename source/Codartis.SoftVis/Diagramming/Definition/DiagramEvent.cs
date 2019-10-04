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
        [NotNull] public IDiagram NewDiagram { get; }
        [NotNull] [ItemNotNull] public IEnumerable<DiagramShapeEventBase> ShapeEvents { get; }

        public DiagramEvent(
            [NotNull] IDiagram newDiagram,
            [NotNull] [ItemNotNull] IEnumerable<DiagramShapeEventBase> shapeEvents = null)
        {
            NewDiagram = newDiagram;
            ShapeEvents = shapeEvents ?? Enumerable.Empty<DiagramShapeEventBase>();
        }

        public static DiagramEvent None([NotNull] IDiagram diagram) => new DiagramEvent(diagram);

        public static DiagramEvent Create(
            [NotNull] IDiagram diagram,
            [NotNull] [ItemNotNull] IEnumerable<DiagramShapeEventBase> shapeEvents)
        {
            return new DiagramEvent(diagram, shapeEvents);
        }

        public static DiagramEvent Create([NotNull] IDiagram diagram, DiagramShapeEventBase shapeEvent = null)
        {
            var shapeEvents = shapeEvent == null
                ? Enumerable.Empty<DiagramShapeEventBase>()
                : new[] { shapeEvent };

            return new DiagramEvent(diagram, shapeEvents);
        }
    }
}