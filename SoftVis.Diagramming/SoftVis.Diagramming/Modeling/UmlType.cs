using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Modeling
{
    public abstract class UmlType : UmlTypeOrPackage
    {
        private UmlType _baseType;
        private UmlGeneralization _baseRelationship;

        public UmlType BaseType
        {
            get { return _baseType; }
            set
            {
                _baseType = value;
                _baseRelationship = _baseType == null
                    ? null
                    :  new UmlGeneralization(this, _baseType);
            }
        }

        public override IEnumerable<UmlRelationship> OutgoingRelationships
        {
            get
            {
                if (_baseRelationship != null)
                    yield return _baseRelationship; 
            }
        }
    }
}
