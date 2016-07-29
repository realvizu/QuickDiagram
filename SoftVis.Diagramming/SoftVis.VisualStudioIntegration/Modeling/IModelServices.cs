using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines model operations for the application commands package.
    /// </summary>
    public interface IModelServices
    {
        /// <summary>
        /// A read-only view of the model.
        /// </summary>
        IReadOnlyModel Model { get; }

        /// <summary>
        /// Gets a model entity from a Roslyn symbol. Creates the entity if not yet exists in the model.
        /// </summary>
        /// <param name="namedTypeSymbol">A Roslyn symbol.</param>
        /// <returns>The model entity that corresponds to the given Roslyn symbol.</returns>
        IRoslynBasedModelEntity FindOrCreateModelEntity(INamedTypeSymbol namedTypeSymbol);

        /// <summary>
        /// Explores related symbols in the Roslyn model and adds them to the model.
        /// </summary>
        /// <param name="modelEntity">The starting model entity.</param>
        /// <param name="relatedEntitySpecification">Optionally specifies what kind of relations should be explored. Null means all relations.</param>
        /// <param name="recursive">True means repeat exploring for related entities. Default is false.</param>
        void ExtendModelWithRelatedEntities(IRoslynBasedModelEntity modelEntity, 
            RelatedEntitySpecification? relatedEntitySpecification = null, bool recursive = false);
    }
}
