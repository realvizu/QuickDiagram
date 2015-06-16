using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Modeling
{
    public abstract class UmlModelVisitorBase<T>
    {
        public T Visit(UmlModelElement item)
        {
            return item.AcceptVisitor(this);
        }

        public virtual T Visit(UmlModel item) { return default(T); }
        public virtual T Visit(UmlPackage item) { return default(T); }
        public virtual T Visit(UmlClass item) { return default(T); }
        public virtual T Visit(UmlInterface item) { return default(T); }
        public virtual T Visit(UmlGeneralization item) { return default(T); }
    }
}
