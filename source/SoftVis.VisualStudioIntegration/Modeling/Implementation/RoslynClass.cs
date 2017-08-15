using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn class symbol.
    /// </summary>
    internal class RoslynClass : RoslynType
    {
        internal RoslynClass(ModelItemId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, TypeKind.Class)
        {
        }

        public override int Priority => 4;

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
                foreach (var baseSymbolRelation in GetBaseTypes(RoslynSymbol))
                    yield return baseSymbolRelation;

            if (entityRelationType == null || entityRelationType == EntityRelationTypes.Subtype)
                foreach (var derivedSymbolRelation in GetDerivedTypes(roslynModelProvider, RoslynSymbol))
                    yield return derivedSymbolRelation;

            if (entityRelationType == null || entityRelationType == RoslynEntityRelationTypes.ImplementedInterface)
                foreach (var implementedSymbolRelation in GetImplementedInterfaces(RoslynSymbol))
                    yield return implementedSymbolRelation;
        }

        private static IEnumerable<RoslynSymbolRelation> GetBaseTypes(INamedTypeSymbol roslynSymbol)
        {
            var baseSymbol = roslynSymbol.BaseType;
            if (baseSymbol?.TypeKind == TypeKind.Class)
                yield return new RoslynSymbolRelation(roslynSymbol, baseSymbol, EntityRelationTypes.BaseType);
        }
    }
}
