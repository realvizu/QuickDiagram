using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn interface symbol.
    /// </summary>
    internal class RoslynInterface : RoslynType
    {
        internal RoslynInterface(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Interface)
        {
        }

        public override int Priority => 2;

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="entityRelationType">Optionally specifies what kind of relations should be found. Null means all relations.</param>
        /// <returns>Related Roslyn symbols.</returns>
        public override IEnumerable<RoslynSymbolRelation> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider,
            EntityRelationType? entityRelationType = null)
        {
            if (entityRelationType == null || entityRelationType == EntityRelationTypes.BaseType)
                foreach (var baseSymbolRelation in GetBaseInterfaces(RoslynSymbol))
                    yield return baseSymbolRelation;

            if (entityRelationType == null || entityRelationType == EntityRelationTypes.Subtype)
                foreach (var derivedSymbolRelation in GetDerivedInterfaces(roslynModelProvider, RoslynSymbol))
                    yield return derivedSymbolRelation;

            if (entityRelationType == null || entityRelationType == RoslynEntityRelationTypes.ImplementerType)
                foreach (var implementingSymbolRelation in GetImplementingTypes(roslynModelProvider, RoslynSymbol))
                    yield return implementingSymbolRelation;
        }

        private static IEnumerable<RoslynSymbolRelation> GetBaseInterfaces(INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementedInterfaceSymbol in interfaceSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RoslynSymbolRelation(interfaceSymbol, implementedInterfaceSymbol, EntityRelationTypes.BaseType);
        }
    }
}
