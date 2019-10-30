using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines Roslyn-specific model operations.
    /// </summary>
    internal interface IRoslynModelService
    {
        // TODO: move to IRoslynDiagramService ?
        bool HideTrivialBaseNodes { get; set; }

        /// <summary>
        /// Returns a value indicating whether the current symbol (the one under the caret in the active source code editor) can be added to the model.
        /// </summary>
        /// <returns>True if there is a symbol under the caret that can be added to the model.</returns>
        [NotNull]
        Task<bool> IsCurrentSymbolAvailableAsync();

        /// <summary>
        /// Adds the current Roslyn symbol (under the caret in the active source code editor) to the model.
        /// </summary>
        /// <returns>The model node that corresponds to the current Roslyn symbol or Nothing if there's no current symbol.</returns>
        [NotNull]
        Task<Maybe<IModelNode>> AddCurrentSymbolAsync();

        /// <summary>
        /// Explores related nodes and adds them to the model.
        /// </summary>
        /// <param name="node">The starting model node.</param>
        /// <param name="directedModelRelationshipType">Optionally specifies what kind of relations should be explored. Null means all relations.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="progress">Optional progress reporting object.</param>
        /// <param name="recursive">True means repeat exploring for related node. Default is false.</param>
        [NotNull]
        Task ExtendModelWithRelatedNodesAsync(
            [NotNull] IModelNode node,
            DirectedModelRelationshipType? directedModelRelationshipType = null,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null,
            bool recursive = false);

        /// <summary>
        /// Returns a value indicating whether a model node has source code.
        /// </summary>
        /// <param name="modelNode">A model node.</param>
        /// <remarks>True if the model node has source code, false otherwise.</remarks>
        [NotNull]
        Task<bool> HasSourceAsync([NotNull]IModelNode modelNode);

        /// <summary>
        /// Shows the source in the host environment that corresponds to the given model node.
        /// </summary>
        /// <param name="modelNode">A model node.</param>
        [NotNull]
        Task ShowSourceAsync([NotNull]IModelNode modelNode);

        /// <summary>
        /// Updates the model from the current source code.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="progress">Optional progress reporting object.</param>
        [NotNull]
        Task UpdateFromSourceAsync(
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);
    }
}