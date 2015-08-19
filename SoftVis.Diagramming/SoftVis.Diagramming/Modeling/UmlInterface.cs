using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Modeling
{
    public class UmlInterface : UmlType
    {
        public override TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
