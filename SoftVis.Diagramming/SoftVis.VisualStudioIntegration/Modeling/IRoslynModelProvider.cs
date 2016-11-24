using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Provides access to the Roslyn model of the host environment.
    /// </summary>
    public interface IRoslynModelProvider
    {
        /// <summary>
        /// Returns the host workspace object that can be used to access the current Roslyn compilation.
        /// </summary>
        /// <returns>The current Roslyn compilation workspace.</returns>
        Workspace GetWorkspace();

        /// <summary>
        /// Returns the Roslyn symbol under the caret in the active source code editor.
        /// </summary>
        /// <returns>The Roslyn symbol under the caret or null.</returns>
        Task<ISymbol> GetCurrentSymbolAsync();

        /// <summary>
        /// Shows the source file in the host environment that corresponds to the given Roslyn symbol.
        /// </summary>
        /// <param name="symbol">A symbol from the source file to be shown.</param>
        void ShowSource(ISymbol symbol);
    }
}
