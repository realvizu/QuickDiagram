using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// The base class for all UML model relationships between a source and a target type or package.
    /// </summary>
    [DebuggerDisplay("{SourceElement}-->{TargetElement}")]
    public abstract class UmlRelationship : UmlModelElement
    {
        public abstract UmlTypeOrPackage SourceElement { get; }
        public abstract UmlTypeOrPackage TargetElement { get; }
    }
}
