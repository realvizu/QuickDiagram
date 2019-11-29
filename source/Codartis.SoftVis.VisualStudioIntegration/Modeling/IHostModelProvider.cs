using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Provides access to the Roslyn model of the host environment.
    /// </summary>
    public interface IHostModelProvider
    {
        /// <summary>
        /// Returns the host workspace object that can be used to access the current Roslyn compilation.
        /// </summary>
        /// <returns>The current Roslyn compilation workspace.</returns>
        [NotNull]
        Task<Workspace> GetWorkspaceAsync();

        /// <summary>
        /// Returns the Roslyn symbol under the caret in the active source code editor.
        /// </summary>
        /// <returns>The Roslyn symbol under the caret or null.</returns>
        [NotNull]
        Task<Maybe<ISymbol>> TryGetCurrentSymbolAsync();

        /// <summary>
        /// Returns the related symbols of a given Roslyn symbol for a kind of relationship type (or all).
        /// </summary>
        [NotNull]
        [ItemNotNull]
        Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(
            [NotNull] ISymbol symbol,
            DirectedModelRelationshipType? directedModelRelationshipType = null);

        /// <summary>
        /// Returns a value indicating whether a Roslyn symbol has source code.
        /// </summary>
        /// <param name="symbol">A Roslyn symbol.</param>
        /// <remarks>True if the Roslyn symbol has source code, false otherwise.</remarks>
        [NotNull]
        Task<bool> HasSourceAsync([NotNull] ISymbol symbol);

        /// <summary>
        /// Shows the source file in the host environment that corresponds to the given Roslyn symbol.
        /// </summary>
        /// <param name="symbol">A Roslyn symbol from the source file to be shown.</param>
        [NotNull]
        Task ShowSourceAsync([NotNull] ISymbol symbol);
    }
}