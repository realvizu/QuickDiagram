using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Implements the IRoslynModelProvider interface by delegating to IHostWorkspaceServices. 
    /// </summary>
    internal class RoslynModelProvider : IRoslynModelProvider
    {
        private readonly IHostWorkspaceServices _hostWorkspaceServices;

        public RoslynModelProvider(IHostWorkspaceServices hostWorkspaceServices)
        {
            _hostWorkspaceServices = hostWorkspaceServices;
        }

        public Workspace GetWorkspace() => _hostWorkspaceServices.GetWorkspace();
    }
}
