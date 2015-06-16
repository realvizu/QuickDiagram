using System;

namespace Codartis.SoftVis.Modeling
{
    public class UmlClass : UmlType
    {
        public override T AcceptVisitor<T>(UmlModelVisitorBase<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
