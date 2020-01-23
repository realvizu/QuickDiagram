using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Editor;
using Document = Microsoft.CodeAnalysis.Document;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;
using Task = System.Threading.Tasks.Task;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Gets information from Visual Studio about the current solution, projects, source documents.
    /// </summary>
    internal class HostWorkspaceGateway : IRoslynModelProvider
    {
        private const string CSharpContentTypeName = "CSharp";

        private readonly IPackageServices _packageServices;

        internal HostWorkspaceGateway(IPackageServices packageServices)
        {
            _packageServices = packageServices;
        }

        public async Task<Workspace> GetWorkspaceAsync()
        {
            return await _packageServices.GetVisualStudioWorkspaceAsync();
        }

        public async Task<ISymbol> GetCurrentSymbolAsync()
        {
            var activeTextView = await GetActiveTextViewAsync();
            if (activeTextView == null)
                return null;

            var document = GetCurrentDocument(activeTextView);
            if (document == null)
                return null;

            var syntaxTree = await document.GetSyntaxTreeAsync();
            var syntaxRoot = await syntaxTree.GetRootAsync();
            var span = GetSelection(activeTextView);
            var currentNode = syntaxRoot.FindNode(span);

            var semanticModel = await document.GetSemanticModelAsync();
            var symbol = GetSymbolForSyntaxNode(semanticModel, currentNode);
            return symbol;
        }

        private static ISymbol GetSymbolForSyntaxNode(SemanticModel semanticModel, SyntaxNode node)
        {
            if (node is TypeDeclarationSyntax ||
                node is EnumDeclarationSyntax ||
                node is DelegateDeclarationSyntax)
                return semanticModel.GetDeclaredSymbol(node);

            var identifierNode = FindSimpleNameSyntax(node);
            return identifierNode == null
                ? null
                : semanticModel.GetSymbolInfo(identifierNode).Symbol;
        }

        private static SimpleNameSyntax FindSimpleNameSyntax(SyntaxNode node)
        {
            var simpleNameSyntax = node as SimpleNameSyntax;
            if (simpleNameSyntax != null)
                return simpleNameSyntax;

            foreach (var childNode in node.ChildNodes())
            {
                simpleNameSyntax = FindSimpleNameSyntax(childNode);
                if (simpleNameSyntax != null)
                    break;
            }

            return simpleNameSyntax;
        }

        public async Task<bool> HasSourceAsync(ISymbol symbol)
        {
            return await GetDocumentIdAsync(symbol) != null;
        }

        public async Task ShowSourceAsync(ISymbol symbol)
        {
            var location = symbol?.Locations.FirstOrDefault();
            if (location == null)
                return;

            var documentId = await GetDocumentIdAsync(symbol);
            if (documentId == null)
                return;

            var workspace = await GetWorkspaceAsync();
            workspace.OpenDocument(documentId, activate: true);

            await SelectSourceLocationAsync(location.GetLineSpan().Span);
        }

        private async Task SelectSourceLocationAsync(LinePositionSpan span)
        {
            var hostService = await _packageServices.GetHostEnvironmentServiceAsync();

            var selection = hostService.ActiveDocument.Selection as TextSelection;
            if (selection == null)
                return;

            selection.MoveTo(span.Start.Line + 1, span.Start.Character + 1, Extend: false);
            selection.MoveTo(span.End.Line + 1, span.End.Character + 1, Extend: true);
        }

        private Document GetCurrentDocument(ITextView textView)
        {
            var currentSnapshot = textView.TextBuffer.CurrentSnapshot;
            var contentType = currentSnapshot.ContentType;
            if (!contentType.IsOfType(CSharpContentTypeName))
                return null;

            return currentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        }

        private async Task<IWpfTextView> GetActiveTextViewAsync()
        {
            var textManager = await _packageServices.GetTextManagerServiceAsync();
            textManager.GetActiveView(1, null, out var textView);
            var editorAdaptersFactoryService = await _packageServices.GetEditorAdaptersFactoryServiceAsync();
            return editorAdaptersFactoryService.GetWpfTextView(textView);
        }

        private async Task<DocumentId> GetDocumentIdAsync(ISymbol symbol)
        {
            var workspace = await GetWorkspaceAsync();

            var location = symbol?.Locations.FirstOrDefault();
            if (location == null)
                return null;

            return workspace?.CurrentSolution?.GetDocumentId(location.SourceTree);
        }

        private TextSpan GetSelection(ITextView activeWpfTextView)
        {
            var visualStudioSpan = activeWpfTextView.Selection.StreamSelectionSpan.SnapshotSpan.Span;
            var roslynSpan = new TextSpan(visualStudioSpan.Start, visualStudioSpan.Length);
            return roslynSpan;
        }
    }
}
