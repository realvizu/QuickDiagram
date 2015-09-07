using System;
using System.Collections.Generic;
using System.Diagnostics;
using Codartis.SoftVis.Modeling;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming
{
    [DebuggerDisplay("{Source}---{Type}-->{Target}")]
    public class DiagramConnector : DiagramShape, IEdge<DiagramNode>
    {
        public DiagramNode Source { get; }
        public DiagramNode Target { get; }
        public DiagramConnectorType Type { get; }
        public IEnumerable<DiagramPoint> RoutePoints { get; set; }

        public DiagramConnector(UmlModelElement modelElement, DiagramNode source, DiagramNode target, DiagramConnectorType type)
            :base(modelElement)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
            Type = type;
        }

        public override string ToString()
        {
            return Source + "---" + Type + "-->" + Target;
        }
    }
}
