using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
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
        public override bool IsAbstract => RoslynSymbol.IsAbstract;

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="entityRelationType">Optionally specifies what kind of relations should be found. Null means all relations.</param>
        /// <returns>Related Roslyn symbols.</returns>
        public override async Task<IEnumerable<RoslynSymbolRelation>> FindRelatedSymbolsAsync(IRoslynModelProvider roslynModelProvider,
            EntityRelationType? entityRelationType = null)
        {
            var result = new List<RoslynSymbolRelation>();

            if (entityRelationType == null || entityRelationType == EntityRelationTypes.BaseType)
                foreach (var baseSymbolRelation in GetBaseTypes(RoslynSymbol))
                    result.Add(baseSymbolRelation);

            if (entityRelationType == null || entityRelationType == EntityRelationTypes.Subtype)
                foreach (var derivedSymbolRelation in await GetDerivedTypesAsync(roslynModelProvider, RoslynSymbol))
                    result.Add(derivedSymbolRelation);

            if (entityRelationType == null || entityRelationType == RoslynEntityRelationTypes.ImplementedInterface)
                foreach (var implementedSymbolRelation in GetImplementedInterfaces(RoslynSymbol))
                    result.Add(implementedSymbolRelation);

            return result;
        }

        private static IEnumerable<RoslynSymbolRelation> GetBaseTypes(INamedTypeSymbol roslynSymbol)
        {
            var baseSymbol = roslynSymbol.BaseType;
            if (baseSymbol?.TypeKind == TypeKind.Class)
                yield return new RoslynSymbolRelation(roslynSymbol, baseSymbol, EntityRelationTypes.BaseType);
        }
    }
}
