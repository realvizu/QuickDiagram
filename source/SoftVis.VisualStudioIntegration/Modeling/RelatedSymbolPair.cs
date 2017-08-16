using System;
using Codartis.SoftVis.Modeling2;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Describes the relationship of 2 roslyn symbols. Immutable.
    /// Converts from base/related to source/target concepts.
    /// </summary>
    public class RelatedSymbolPair
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

        public RelatedSymbolPair(ISymbol baseSymbol, ISymbol relatedSymbol, DirectedModelRelationshipType directedRelationshipType)
        {
            BaseSymbol = baseSymbol;
            RelatedSymbol = relatedSymbol;
            DirectedRelationshipType = directedRelationshipType;
        }

        public Type Type => DirectedRelationshipType.Type;

        /// <summary>
        /// The roslyn symbol that is on the source side of the directed relationship.
        /// </summary>
        public ISymbol SourceSymbol
            => DirectedRelationshipType.Direction == RelationshipDirection.Outgoing
                ? BaseSymbol
                : RelatedSymbol;

        /// <summary>
        /// The roslyn symbol that is on the target side of the directed relationship.
        /// </summary>
        public ISymbol TargetSymbol
            => DirectedRelationshipType.Direction == RelationshipDirection.Outgoing
                ? RelatedSymbol
                : BaseSymbol;

        public RelatedSymbolPair WithRelatedSymbol(ISymbol newRelatedSymbol) 
            => new RelatedSymbolPair(BaseSymbol, newRelatedSymbol, DirectedRelationshipType);

        public bool Matches(IModelRelationship relationship)
        {
            return relationship.GetType() == Type
                   && ((IRoslynModelNode) relationship.Source).SymbolEquals(SourceSymbol)
                   && ((IRoslynModelNode) relationship.Target).SymbolEquals(TargetSymbol);
        }
    }
}
