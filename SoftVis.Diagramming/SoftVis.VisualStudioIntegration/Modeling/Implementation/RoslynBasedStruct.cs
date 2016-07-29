using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model entity created from a Roslyn struct symbol.
    /// </summary>
    internal class RoslynBasedStruct : RoslynBasedModelEntity
    {
        internal RoslynBasedStruct(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Struct)
        {
        }

        public override int Priority => 3;

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="relatedEntitySpecification">Optionally specifies what kind of relations should be found. Null means all relations.</param>
        /// <returns>Related Roslyn symbols.</returns>
        public override IEnumerable<RoslynSymbolRelation> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider,
            RelatedEntitySpecification? relatedEntitySpecification = null)
        {
            if (RoslynRelatedEntitySpecifications.ImplementedInterface.IsSpecifiedBy(relatedEntitySpecification))
                foreach (var implementedSymbolRelation in GetImplementedInterfaces(RoslynSymbol))
                    yield return implementedSymbolRelation;
        }
    }
}
