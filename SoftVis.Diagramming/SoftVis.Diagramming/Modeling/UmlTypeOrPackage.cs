using System.Collections.Generic;
using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    [DebuggerDisplay("TypeOrPackage: {Name}")]
    public abstract class UmlTypeOrPackage : UmlModelElement
    {
        protected string _name;

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public abstract IEnumerable<UmlRelationship> OutgoingRelationships { get; }
    }
}
