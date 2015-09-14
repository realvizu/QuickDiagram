using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace Codartis.SoftVis.VisualStudioIntegration
{
    /// <summary>
    /// Operations to access services from the host environment.
    /// </summary>
    internal interface IHostServiceProvider
    {
        IVsRunningDocumentTable GetRunningDocumentTableService();
        VisualStudioWorkspace GetVisualStudioWorkspace();
    }
}