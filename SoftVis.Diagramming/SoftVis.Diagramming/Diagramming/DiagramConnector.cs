using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming
{
    [DebuggerDisplay("{Source}---{Type}-->{Target}")]
    public class DiagramConnector : DiagramShape, IEdge<DiagramNode>
    {
        public DiagramNode Source { get; set; }
        public DiagramNode Target { get; set; }
        public DiagramConnectorType Type { get; set; }

        public DiagramConnector(DiagramNode source, DiagramNode target, DiagramConnectorType type) 
        {
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
