using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A UML package contains a set of types and nested packages.
    /// </summary>
    [DebuggerDisplay("Count={_elements.Count}")]
    public class UmlPackage : UmlTypeOrPackage, IEnumerable<UmlTypeOrPackage>
    {
        private HashSet<UmlTypeOrPackage> _elements = new HashSet<UmlTypeOrPackage>();

        public void Add(UmlTypeOrPackage element)
        {
            _elements.Add(element);
        }

        public void Remove(UmlTypeOrPackage element)
        {
            _elements.Remove(element);
        }

        public IEnumerable<UmlType> Types
        {
            get { return _elements.OfType<UmlType>(); }
        }

        public IEnumerable<UmlPackage> Packages
        {
            get { return _elements.OfType<UmlPackage>(); }
        }

        public IEnumerator<UmlTypeOrPackage> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override IEnumerable<UmlRelationship> OutgoingRelationships
        {
            get
            {
                return Enumerable.Empty<UmlRelationship>();
            }
        }

        public override TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
