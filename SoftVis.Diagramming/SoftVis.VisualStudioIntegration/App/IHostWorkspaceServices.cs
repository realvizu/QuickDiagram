using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Operations to access the workspace operations of the host environment.
    /// </summary>
    /// <remarks>
    /// Workspace means the current solution, projects, source documents.
    /// </remarks>
    public interface IHostWorkspaceServices
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
        /// <remarks>Asnyc method.</remarks>
        Task<ISymbol> GetCurrentSymbol();

        /// <summary>
        /// Shows the source file in the host environment that corresponds to the given Roslyn symbol.
        /// </summary>
        /// <param name="symbol">A symbol from the source file to be shown.</param>
        void ShowSourceFile(ISymbol symbol);
    }
}