using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Provides access to the Roslyn model.
    /// </summary>
    public interface IRoslynModelProvider
    {
        Workspace GetWorkspace();
    }
}
