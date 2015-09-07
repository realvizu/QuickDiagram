using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    [DebuggerDisplay("{SpecificElement.Name}---Generalization-->{GeneralElement.Name}")]
    public class UmlGeneralization : UmlRelationship
    {
        public UmlType SpecificElement { get; }
        public UmlType GeneralElement { get; }

        public UmlGeneralization(UmlType specificElement, UmlType generalElement)
        {
            SpecificElement = specificElement;
            GeneralElement = generalElement;
        }

        public override UmlTypeOrPackage SourceElement => SpecificElement;
        public override UmlTypeOrPackage TargetElement => GeneralElement;

        public override TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
