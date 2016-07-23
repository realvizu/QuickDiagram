using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Describes the relationship of 2 roslyn type symbols.
    /// </summary>
    public struct RelatedRoslynSymbols
    {
        /// <summary>
        /// A roslyn type symbol whose related symbol is described.
        /// </summary>
        public INamedTypeSymbol BaseSymbol { get; }
        /// <summary>
        /// A roslyn symbol that is related to the base symbol.
        /// </summary>
        public INamedTypeSymbol RelatedSymbol { get; }
        /// <summary>
        /// Specifies the relationship of the base and related symbols.
        /// </summary>
        public RelationshipSpecification RelationshipSpecification { get; }

        public RelatedRoslynSymbols(INamedTypeSymbol baseSymbol, INamedTypeSymbol relatedSymbol, RelationshipSpecification relationshipSpecification)
        {
            BaseSymbol = baseSymbol;
            RelatedSymbol = relatedSymbol;
            RelationshipSpecification = relationshipSpecification;
        }

        /// <summary>
        /// The roslyn type symbol that is on the source side of the directed relationship.
        /// </summary>
        public INamedTypeSymbol SourceSymbol
            => RelationshipSpecification.Direction == ModelRelationshipDirection.Outgoing
                ? BaseSymbol
                : RelatedSymbol;

        /// <summary>
        /// The roslyn type symbol that is on the target side of the directed relationship.
        /// </summary>
        public INamedTypeSymbol TargetSymbol
            => RelationshipSpecification.Direction == ModelRelationshipDirection.Outgoing
                ? RelatedSymbol
                : BaseSymbol;
    }
}
