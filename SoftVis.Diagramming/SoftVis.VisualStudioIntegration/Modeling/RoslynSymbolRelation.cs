using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Describes the relationship of 2 roslyn type symbols.
    /// </summary>
    public class RoslynSymbolRelation
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
        public RelatedEntitySpecification RelatedEntitySpecification { get; }

        public RoslynSymbolRelation(INamedTypeSymbol baseSymbol, INamedTypeSymbol relatedSymbol, RelatedEntitySpecification relatedEntitySpecification)
        {
            BaseSymbol = baseSymbol;
            RelatedSymbol = relatedSymbol;
            RelatedEntitySpecification = relatedEntitySpecification;
        }

        public ModelRelationshipTypeSpecification TypeSpecification => RelatedEntitySpecification.TypeSpecification;

        /// <summary>
        /// The roslyn type symbol that is on the source side of the directed relationship.
        /// </summary>
        public INamedTypeSymbol SourceSymbol
            => RelatedEntitySpecification.Direction == EntityRelationDirection.Outgoing
                ? BaseSymbol
                : RelatedSymbol;

        /// <summary>
        /// The roslyn type symbol that is on the target side of the directed relationship.
        /// </summary>
        public INamedTypeSymbol TargetSymbol
            => RelatedEntitySpecification.Direction == EntityRelationDirection.Outgoing
                ? RelatedSymbol
                : BaseSymbol;
    }
}
