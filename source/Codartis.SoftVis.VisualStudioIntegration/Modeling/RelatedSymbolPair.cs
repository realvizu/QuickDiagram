using System;
using System.Collections.Generic;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Describes the relationship of 2 roslyn symbols.
    /// Immutable.
    /// Converts from base/related to source/target concepts.
    /// </summary>
    public struct RelatedSymbolPair : IEquatable<RelatedSymbolPair>
    {
        [NotNull] private static readonly IEqualityComparer<ISymbol> SymbolComparer = new SymbolEqualityComparer();

        /// <summary>
        /// A roslyn symbol whose related symbol is described.
        /// </summary>
        [NotNull]
        public ISymbol BaseSymbol { get; }

        /// <summary>
        /// A roslyn symbol that is related to the base symbol.
        /// </summary>
        [NotNull]
        public ISymbol RelatedSymbol { get; }

        /// <summary>
        /// Specifies the directed relationship of the base and related symbols.
        /// </summary>
        public DirectedModelRelationshipType DirectedRelationshipType { get; }

        public RelatedSymbolPair(
            [NotNull] ISymbol baseSymbol,
            [NotNull] ISymbol relatedSymbol,
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
        [NotNull]
        public ISymbol SourceSymbol
            => DirectedRelationshipType.Direction == EdgeDirection.Out
                ? BaseSymbol
                : RelatedSymbol;

        /// <summary>
        /// The roslyn symbol that is on the target side of the directed relationship.
        /// </summary>
        [NotNull]
        public ISymbol TargetSymbol
            => DirectedRelationshipType.Direction == EdgeDirection.Out
                ? RelatedSymbol
                : BaseSymbol;

        public RelatedSymbolPair WithRelatedSymbol([NotNull] ISymbol newRelatedSymbol)
            => new RelatedSymbolPair(BaseSymbol, newRelatedSymbol, DirectedRelationshipType);

        public override string ToString() => $"{SourceSymbol.Name}--{Stereotype}->{TargetSymbol.Name}";

        public bool Equals(RelatedSymbolPair other)
        {
            return SymbolComparer.Equals(SourceSymbol, other.SourceSymbol) &&
                   SymbolComparer.Equals(TargetSymbol, other.TargetSymbol) &&
                   Stereotype == other.Stereotype;
        }

        public override bool Equals(object obj)
        {
            return obj is RelatedSymbolPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SymbolComparer.GetHashCode(SourceSymbol);
                hashCode = (hashCode * 397) ^ SymbolComparer.GetHashCode(TargetSymbol);
                hashCode = (hashCode * 397) ^ Stereotype.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(RelatedSymbolPair left, RelatedSymbolPair right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RelatedSymbolPair left, RelatedSymbolPair right)
        {
            return !left.Equals(right);
        }
    }
}