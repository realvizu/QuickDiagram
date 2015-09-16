using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model entity created from a Roslyn interface symbol.
    /// </summary>
    internal class RoslynBasedInterface : RoslynBasedModelEntity
    {
        internal RoslynBasedInterface(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Interface)
        {
        }

        public IEnumerable<RoslynBasedInterface> BaseInterfaces
        {
            get
            {
                return OutgoingRelationships.Where(i => i.IsGeneralization())
                    .Select(i => i.Target).OfType<RoslynBasedInterface>();
            }
        }

        public IEnumerable<RoslynBasedInterface> DerivedInterfaces
        {
            get
            {
                return IncomingRelationships.Where(i => i.IsGeneralization())
                    .Select(i => i.Source).OfType<RoslynBasedInterface>();
            }
        }

        public IEnumerable<RoslynBasedModelEntity> ImplementerTypes
        {
            get
            {
                return IncomingRelationships.Where(i => i.IsInterfaceImplementation())
                    .Select(i => i.Source).OfType<RoslynBasedModelEntity>();
            }
        }
    }
}
