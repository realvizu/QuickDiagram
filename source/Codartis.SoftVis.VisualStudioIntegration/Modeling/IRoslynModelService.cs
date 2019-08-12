using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines model operations for the application commands package.
    /// </summary>
    internal interface IRoslynModelService : IModelService
    {
        bool HideTrivialBaseNodes { get; set; }

        /// <summary>
        /// Returns a value indicating whether the current symbol (the one under the caret in the active source code editor) can be added to the model.
        /// </summary>
        /// <returns>True if there is a symbol under the caret that can be added to the model.</returns>
        Task<bool> IsCurrentSymbolAvailableAsync();

        /// <summary>
        /// Adds the current Roslyn symbol (under the caret in the active source code editor) to the model.
        /// </summary>
        /// <returns>The model node that corresponds to the current Roslyn symbol.</returns>
        Task<IRoslynModelNode> AddCurrentSymbolAsync();

        /// <summary>
        /// Explores related nodes and adds them to the model.
        /// </summary>
        /// <param name="modelNode">The starting model node.</param>
        /// <param name="directedModelRelationshipType">Optionally specifies what kind of relations should be explored. Null means all relations.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="progress">Optional progress reporting object.</param>
        /// <param name="recursive">True means repeat exploring for related node. Default is false.</param>
        Task ExtendModelWithRelatedNodesAsync(IModelNode modelNode, DirectedModelRelationshipType? directedModelRelationshipType = null, 
            CancellationToken cancellationToken = default, IIncrementalProgress progress = null, bool recursive = false);

        /// <summary>
        /// Returns a value indicating whether a model node has source code.
        /// </summary>
        /// <param name="modelNode">A model node.</param>
        /// <remarks>True if the model node has source code, false otherwise.</remarks>
        Task<bool> HasSourceAsync(IRoslynModelNode modelNode);

        /// <summary>
        /// Shows the source in the host environment that corresponds to the given model node.
        /// </summary>
        /// <param name="modelNode">A model node.</param>
        Task ShowSourceAsync(IRoslynModelNode modelNode);

        /// <summary>
        /// Updates the model from the current source code.
        /// </summary>
        /// <param name="visibleModelNodeIds">The IDs of those model nodes that are visible on any of the diagrams.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="progress">Optional progress reporting object.</param>
        Task UpdateFromSourceAsync(
            IEnumerable<ModelNodeId> visibleModelNodeIds,
            CancellationToken cancellationToken = default, 
            IIncrementalProgress progress = null);
    }
}
