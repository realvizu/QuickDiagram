using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    public interface ISourceDocumentProvider
    {
        Document GetCurrentDocument();
        TextSpan GetSelection();
        Workspace GetWorkspace();
    }
}