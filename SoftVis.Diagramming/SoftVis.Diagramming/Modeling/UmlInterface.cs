using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Modeling
{
    public class UmlInterface : UmlType
    {
        public override T AcceptVisitor<T>(UmlModelVisitorBase<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
