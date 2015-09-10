using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Defines operations for accessing the source documents.
    /// </summary>
    internal interface ISourceDocumentProvider
    {
        Document GetCurrentDocument();
        TextSpan GetSelection();
        Workspace GetWorkspace();
    }
}