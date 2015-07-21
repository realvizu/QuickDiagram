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
        private DiagramRect _rect;

        public DiagramNode Source { get; private set; }
        public DiagramNode Target { get; private set; }
        public DiagramConnectorType Type { get; private set; }
        public IEnumerable<DiagramPoint> RoutePoints { get; set; }

        public DiagramConnector(UmlModelElement modelElement, DiagramNode source, DiagramNode target, DiagramConnectorType type)
            :base(modelElement)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            Source = source;
            Target = target;
            Type = type;
            RecalculatePosition();
        }

        public override DiagramRect Rect
        {
            get { return _rect; }
        }

        internal void RecalculatePosition()
        {
            _rect = DiagramRect.Union(Source.Rect, Target.Rect);
        }

        public override string ToString()
        {
            return Source + "---" + Type + "-->" + Target;
        }
    }
}
