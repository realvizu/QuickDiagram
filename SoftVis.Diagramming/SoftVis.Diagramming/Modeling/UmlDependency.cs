using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    [DebuggerDisplay("{DependantElement.Name}---DependsOn-->{DependsOnElement.Name}")]
    public class UmlDependency : UmlRelationship
    {
        public UmlType DependantElement { get; }
        public UmlType DependsOnElement { get; }

        public UmlDependency(UmlType dependantElement, UmlType dependsOnElement)
        {
            DependantElement = dependantElement;
            DependsOnElement = dependsOnElement;
        }

        public override UmlTypeOrPackage SourceElement => DependantElement;
        public override UmlTypeOrPackage TargetElement => DependsOnElement;

        public override TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
