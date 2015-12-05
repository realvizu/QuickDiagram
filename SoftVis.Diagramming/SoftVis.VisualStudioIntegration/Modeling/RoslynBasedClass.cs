using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model entity created from a Roslyn class symbol.
    /// </summary>
    internal class RoslynBasedClass : RoslynBasedModelEntity
    {
        internal RoslynBasedClass(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Class)
        {
        }

        public override int Priority => 4;

        public bool IsAbstract => RoslynSymbol.IsAbstract;

        public RoslynBasedClass BaseClass
        {
            get
            {
                return OutgoingRelationships.FirstOrDefault(i => i.IsGeneralization())?.Target as RoslynBasedClass;
            }
        }

        public IEnumerable<RoslynBasedInterface> ImplementedInterfaces
        {
            get
            {
                return OutgoingRelationships.Where(i => i.IsInterfaceImplementation())
                    .Select(i => i.Target).OfType<RoslynBasedInterface>();
            }
        }

        public IEnumerable<RoslynBasedClass> DerivedClasses
        {
            get
            {
                return IncomingRelationships.Where(i => i.IsGeneralization())
                    .Select(i => i.Source).OfType<RoslynBasedClass>();
            }
        }
    }
}
