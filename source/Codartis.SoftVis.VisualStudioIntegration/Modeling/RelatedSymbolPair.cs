using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling.Definition;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Describes the relationship of 2 roslyn symbols.
    /// Immutable.
    /// Converts from base/related to source/target concepts.
    /// </summary>
    public struct RelatedSymbolPair
    {
        /// <summary>
        /// A roslyn symbol whose related symbol is described.
        /// </summary>
        public ISymbol BaseSymbol { get; }

        /// <summary>
        /// A roslyn symbol that is related to the base symbol.
        /// </summary>
        public ISymbol RelatedSymbol { get; }

        /// <summary>
        /// Specifies the directed relationship of the base and related symbols.
        /// </summary>
        public DirectedModelRelationshipType DirectedRelationshipType { get; }

        public RelatedSymbolPair(
            ISymbol baseSymbol,
            ISymbol relatedSymbol,
            DirectedModelRelationshipType directedRelationshipType)
        {
            BaseSymbol = baseSymbol;
            RelatedSymbol = relatedSymbol;
            DirectedRelationshipType = directedRelationshipType;
        }

        public ModelRelationshipStereotype Stereotype => DirectedRelationshipType.Stereotype;

        /// <summary>
        /// The roslyn symbol that is on the source side of the directed relationship.
        /// </summary>
        public ISymbol SourceSymbol
            => DirectedRelationshipType.Direction == EdgeDirection.Out
                ? BaseSymbol
                : RelatedSymbol;

        /// <summary>
        /// The roslyn symbol that is on the target side of the directed relationship.
        /// </summary>
        public ISymbol TargetSymbol
            => DirectedRelationshipType.Direction == EdgeDirection.Out
                ? RelatedSymbol
                : BaseSymbol;

        public RelatedSymbolPair WithRelatedSymbol(ISymbol newRelatedSymbol) => new RelatedSymbolPair(BaseSymbol, newRelatedSymbol, DirectedRelationshipType);

        public override string ToString()
        {
            return $"{BaseSymbol.Name}--{DirectedRelationshipType}->{RelatedSymbol.Name}";
        }
    }
}