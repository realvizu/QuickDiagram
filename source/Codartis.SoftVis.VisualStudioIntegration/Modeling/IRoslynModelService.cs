using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines Roslyn-specific model operations.
    /// </summary>
    internal interface IRoslynModelService
    {
        /// <summary>
        /// Controls whether trivial types like object can be added to the model.
        /// </summary>
        bool ExcludeTrivialTypes { get; set; }

        /// <summary>
        /// Creates a model node from a roslyn symbol and adds it to the model or just returns it if the model already contains it.
        /// </summary>
        [NotNull]
        IModelNode AddSymbol([NotNull] ISymbol symbol);

        /// <summary>
        /// Adds the current Roslyn symbol (under the caret in the active source code editor) to the model.
        /// </summary>
        /// <returns>The model node that corresponds to the current Roslyn symbol or Nothing if there's no current symbol.</returns>
        //[NotNull]
        //Task<Maybe<IModelNode>> AddCurrentSymbolAsync();

        /// <summary>
        /// Explores related nodes and adds them to the model.
        /// </summary>
        /// <param name="node">The starting model node.</param>
        /// <param name="directedModelRelationshipType">Optionally specifies what kind of relations should be explored. Null means all relations.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="progress">Optional progress reporting object.</param>
        /// <param name="recursive">True means repeat exploring for related node. Default is false.</param>
        //[NotNull]
        //Task ExtendModelWithRelatedNodesAsync(
        //    [NotNull] IModelNode node,
        //    DirectedModelRelationshipType? directedModelRelationshipType = null,
        //    CancellationToken cancellationToken = default,
        //    IIncrementalProgress progress = null,
        //    bool recursive = false);

        /// <summary>
        /// Returns a value indicating whether a model node has source code.
        /// </summary>
        /// <param name="modelNode">A model node.</param>
        /// <remarks>True if the model node has source code, false otherwise.</remarks>
        //[NotNull]
        //Task<bool> HasSourceAsync([NotNull]IModelNode modelNode);

        /// <summary>
        /// Shows the source in the host environment that corresponds to the given model node.
        /// </summary>
        /// <param name="modelNode">A model node.</param>
        //[NotNull]
        //Task ShowSourceAsync([NotNull]IModelNode modelNode);

        /// <summary>
        /// Updates the model from the current source code.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="progress">Optional progress reporting object.</param>
        //[NotNull]
        //Task UpdateFromSourceAsync(
        //    CancellationToken cancellationToken = default,
        //    IIncrementalProgress progress = null);
    }
}