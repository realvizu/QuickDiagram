using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// The base class for all UML model relationships between a source and a target type or package.
    /// </summary>
    [DebuggerDisplay("{SourceElement.Name}-->{TargetElement.Name}")]
    public abstract class UmlRelationship : UmlModelElement
    {
        public abstract UmlTypeOrPackage SourceElement { get; }
        public abstract UmlTypeOrPackage TargetElement { get; }
    }
}
