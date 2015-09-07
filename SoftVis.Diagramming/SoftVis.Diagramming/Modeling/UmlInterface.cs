using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    [DebuggerDisplay("Interface: {Name}")]
    public class UmlInterface : UmlType
    {
        public override TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
