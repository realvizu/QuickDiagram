using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Modeling
{
    [DebuggerDisplay("{Name}")]
    public abstract class UmlTypeOrPackage : UmlModelElement
    {
        public virtual string Name { get; set; }

        public abstract IEnumerable<UmlRelationship> OutgoingRelationships { get; }
    }
}
