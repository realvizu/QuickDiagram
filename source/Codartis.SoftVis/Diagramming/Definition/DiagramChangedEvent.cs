using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Describes a change in a diagram by presenting the new diagram state and the changes that led to it.
    /// </summary>
    public struct DiagramChangedEvent
    {
        [NotNull] public IDiagram NewDiagram { get; }
        [NotNull] [ItemNotNull] public IEnumerable<DiagramComponentChangedEventBase> ComponentChanges { get; }

        public DiagramChangedEvent(
            [NotNull] IDiagram newDiagram,
            [NotNull] [ItemNotNull] IEnumerable<DiagramComponentChangedEventBase> componentChanges = null)
        {
            NewDiagram = newDiagram;
            ComponentChanges = componentChanges ?? Enumerable.Empty<DiagramComponentChangedEventBase>();
        }

        public static DiagramChangedEvent None([NotNull] IDiagram diagram) => new DiagramChangedEvent(diagram);

        public static DiagramChangedEvent Create(
            [NotNull] IDiagram diagram,
            [NotNull] [ItemNotNull] IEnumerable<DiagramComponentChangedEventBase> componentChangedEvents)
        {
            return new DiagramChangedEvent(diagram, componentChangedEvents);
        }

        public static DiagramChangedEvent Create([NotNull] IDiagram diagram, DiagramComponentChangedEventBase componentChangedEvent = null)
        {
            var componentChanges = componentChangedEvent == null
                ? Enumerable.Empty<DiagramComponentChangedEventBase>()
                : new[] { componentChangedEvent };

            return new DiagramChangedEvent(diagram, componentChanges);
        }
    }
}