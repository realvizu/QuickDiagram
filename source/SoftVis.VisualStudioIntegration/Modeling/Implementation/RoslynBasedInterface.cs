using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
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

        public override int Priority => 2;

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
                foreach (var baseSymbolRelation in GetBaseInterfaces(RoslynSymbol))
                    result.Add(baseSymbolRelation);

            if (entityRelationType == null || entityRelationType == EntityRelationTypes.Subtype)
                foreach (var derivedSymbolRelation in await GetDerivedInterfacesAsync(roslynModelProvider, RoslynSymbol))
                    result.Add(derivedSymbolRelation);

            if (entityRelationType == null || entityRelationType == RoslynEntityRelationTypes.ImplementerType)
                foreach (var implementingSymbolRelation in await GetImplementingTypesAsync(roslynModelProvider, RoslynSymbol))
                    result.Add(implementingSymbolRelation);

            return result;
        }

        private static IEnumerable<RoslynSymbolRelation> GetBaseInterfaces(INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementedInterfaceSymbol in interfaceSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RoslynSymbolRelation(interfaceSymbol, implementedInterfaceSymbol, EntityRelationTypes.BaseType);
        }
    }
}
