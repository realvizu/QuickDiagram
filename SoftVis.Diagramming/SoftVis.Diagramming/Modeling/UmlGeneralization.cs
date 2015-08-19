using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Modeling
{
    [DebuggerDisplay("{SpecificElement}---Generalization-->{GeneralElement}")]
    public class UmlGeneralization : UmlRelationship
    {
        public UmlType SpecificElement { get; private set; }
        public UmlType GeneralElement { get; private set; }

        public UmlGeneralization(UmlType specificElement, UmlType generalElement)
        {
            SpecificElement = specificElement;
            GeneralElement = generalElement;
        }

        public override UmlTypeOrPackage SourceElement
        {
            get { return SpecificElement; }
        }

        public override UmlTypeOrPackage TargetElement
        {
            get { return GeneralElement; }
        }

        public override TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
