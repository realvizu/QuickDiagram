using System;

namespace Codartis.SoftVis.Modeling
{
    public class UmlClass : UmlType
    {
        public override TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
