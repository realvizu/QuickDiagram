using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Connects our own model with the underlying Roslyn model.
    /// </summary>
    internal interface IRoslynBasedModelService
    {
        [NotNull] IModel LatestModel { get; }

        /// <summary>
        /// Returns the Roslyn symbol for a given model node.
        /// </summary>
        [NotNull]
        ISymbol GetSymbol([NotNull] IModelNode modelNode);

        /// <summary>
        /// Returns a value indicating whether the given symbol can be added to the model.
        /// </summary>
        bool CanAddSymbol([NotNull] ISymbol symbol);

        /// <summary>
        /// Creates a model node from a roslyn symbol and adds it to the model or just returns it if the model already contains it.
        /// Returns Maybe<see cref="Maybe{T}.Nothing"/> if the symbol is not modeled.
        /// </summary>
        Maybe<IModelNode> TryGetOrAddNode([NotNull] ISymbol symbol);

        /// <summary>
        /// Creates a model relationship from 2 related roslyn symbols and adds it to the model or just returns it if the model already contains it.
        /// </summary>
        [NotNull]
        IModelRelationship GetOrAddRelationship(RelatedSymbolPair relatedSymbolPair);

        /// <summary>
        /// Explores related nodes and adds them to the model.
        /// </summary>
        /// <param name="node">The starting model node.</param>
        /// <param name="directedModelRelationshipType">Optionally specifies what kind of relations should be explored. Null means all relations.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="progress">Optional progress reporting object.</param>
        /// <param name="recursive">True means repeat exploring for related node. Default is false.</param>
        // TODO: not SRP, move it into a ModelExtender class
        [NotNull]
        Task ExtendModelWithRelatedNodesAsync(
            [NotNull] IModelNode node,
            DirectedModelRelationshipType? directedModelRelationshipType = null,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null,
            bool recursive = false);

        /// <summary>
        /// Updates the model from the current source code.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="progress">Optional progress reporting object.</param>
        // TODO: not SRP, move it into a ModelUpdater class
        [NotNull]
        Task UpdateFromSourceAsync(
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);

        void ClearModel();
    }
}