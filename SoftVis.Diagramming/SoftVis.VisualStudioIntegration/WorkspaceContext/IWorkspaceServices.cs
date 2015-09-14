using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.WorkspaceContext
{
    /// <summary>
    /// Defines workspace operations (on solution, projects, source documents) for the application commands and modeling packages.
    /// </summary>
    public interface IWorkspaceServices
    {
        Workspace GetWorkspace();
        Task<ISymbol> GetCurrentSymbol();
        void ShowSourceFile(ISymbol symbol);
    }
}