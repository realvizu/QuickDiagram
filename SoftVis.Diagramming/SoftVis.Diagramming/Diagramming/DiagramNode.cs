using System.Diagnostics;

namespace Codartis.SoftVis.Diagramming
{
    [DebuggerDisplay("{Name}")]
    public abstract class DiagramNode : DiagramShape
    {
        public DiagramPoint Position { get; set; }
        public DiagramSize Size { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
