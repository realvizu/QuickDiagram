using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Wraps a model service with Roslyn-specific operations.
    /// </summary>
    internal interface IRoslynModelService
    {
        [NotNull]
        IModel LatestModel { get; }

        /// <summary>
        /// Controls whether trivial types like object can be added to the model.
        /// </summary>
        bool ExcludeTrivialTypes { get; set; }

        /// <summary>
        /// Returns the Roslyn symbol fot a given model node.
        /// </summary>
        [NotNull]
        ISymbol GetSymbol([NotNull] IModelNode modelNode);

        /// <summary>
        /// Creates a model node from a roslyn symbol and adds it to the model or just returns it if the model already contains it.
        /// </summary>
        [NotNull]
        IModelNode GetOrAddNode([NotNull] ISymbol symbol);

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
        [NotNull]
        Task UpdateFromSourceAsync(
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);

        void ClearModel();
    }
}