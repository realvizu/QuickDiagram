using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
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
        public EntityRelationType EntityRelationType { get; }

        public RoslynSymbolRelation(INamedTypeSymbol baseSymbol, INamedTypeSymbol relatedSymbol, EntityRelationType entityRelationType)
        {
            BaseSymbol = baseSymbol;
            RelatedSymbol = relatedSymbol;
            EntityRelationType = entityRelationType;
        }

        public ModelRelationshipType Type => EntityRelationType.Type;

        /// <summary>
        /// The roslyn type symbol that is on the source side of the directed relationship.
        /// </summary>
        public INamedTypeSymbol SourceSymbol
            => EntityRelationType.Direction == EntityRelationDirection.Outgoing
                ? BaseSymbol
                : RelatedSymbol;

        /// <summary>
        /// The roslyn type symbol that is on the target side of the directed relationship.
        /// </summary>
        public INamedTypeSymbol TargetSymbol
            => EntityRelationType.Direction == EntityRelationDirection.Outgoing
                ? RelatedSymbol
                : BaseSymbol;

        public RoslynSymbolRelation WithRelatedSymbol(INamedTypeSymbol newRelatedSymbol) 
            => new RoslynSymbolRelation(BaseSymbol, newRelatedSymbol, EntityRelationType);

        public bool Matches(ModelRelationship relationship)
        {
            return relationship.Type == Type
                   && SourceSymbol.OriginalDefinition.SymbolEquals(((IRoslynBasedModelEntity) relationship.Source).RoslynSymbol.OriginalDefinition)
                   && TargetSymbol.OriginalDefinition.SymbolEquals(((IRoslynBasedModelEntity) relationship.Target).RoslynSymbol.OriginalDefinition);
        }
    }
}
