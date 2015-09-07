using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    [DebuggerDisplay("Class: {Name}")]
    public class UmlClass : UmlType
    {
        public override TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
