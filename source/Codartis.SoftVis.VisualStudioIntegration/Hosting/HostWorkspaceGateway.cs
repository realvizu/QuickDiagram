using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Document = Microsoft.CodeAnalysis.Document;
using Task = System.Threading.Tasks.Task;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Gets information from Visual Studio about the current solution, projects and source documents.
    /// </summary>
    internal sealed class HostWorkspaceGateway : IHostModelProvider
    {
        private const string CSharpContentTypeName = "CSharp";

        private readonly IVisualStudioServices _visualStudioServices;

        public HostWorkspaceGateway(IVisualStudioServices visualStudioServices)
        {
            _visualStudioServices = visualStudioServices;
        }

        public async Task<Workspace> GetWorkspaceAsync()
        {
            return await _visualStudioServices.GetVisualStudioWorkspaceAsync();
        }

        public async Task<Maybe<ISymbol>> TryGetCurrentSymbolAsync()
        {
            var activeTextView = await GetActiveTextViewAsync();
            if (activeTextView == null)
                return Maybe<ISymbol>.Nothing;

            var document = GetCurrentDocument(activeTextView);
            if (document == null)
                return Maybe<ISymbol>.Nothing;

            var syntaxTree = await document.GetSyntaxTreeAsync();
            var syntaxRoot = await syntaxTree.GetRootAsync();
            var span = GetSelection(activeTextView);
            var currentNode = syntaxRoot.FindNode(span);

            var semanticModel = await document.GetSemanticModelAsync();
            var symbol = GetSymbolForSyntaxNode(semanticModel, currentNode);
            Debug.WriteLine($"symbol={symbol}");
            return Maybe.Create(symbol);
        }

        public Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(ISymbol symbol, DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            return Task.FromResult(Enumerable.Empty<RelatedSymbolPair>());
        }

        public async Task<bool> HasSourceAsync(ISymbol symbol)
        {
            return await GetDocumentIdAsync(symbol) != null;
        }

        public async Task ShowSourceAsync(ISymbol symbol)
        {
            var location = symbol.Locations.FirstOrDefault();
            if (location == null)
                return;

            var documentId = await GetDocumentIdAsync(symbol);
            if (documentId == null)
                return;

            var workspace = await GetWorkspaceAsync();
            workspace.OpenDocument(documentId, activate: true);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            SelectSourceLocation(location.GetLineSpan().Span);
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

        private void SelectSourceLocation(LinePositionSpan span)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var hostService = _visualStudioServices.GetHostEnvironmentService();
            var selection = hostService.ActiveDocument.Selection as TextSelection;
            if (selection == null)
                return;

            selection.MoveTo(span.Start.Line + 1, span.Start.Character + 1, Extend: false);
            selection.MoveTo(span.End.Line + 1, span.End.Character + 1, Extend: true);
        }

        private static Document GetCurrentDocument(ITextView textView)
        {
            var currentSnapshot = textView.TextBuffer.CurrentSnapshot;
            var contentType = currentSnapshot.ContentType;
            if (!contentType.IsOfType(CSharpContentTypeName))
                return null;

            return currentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        }

        private async Task<IWpfTextView> GetActiveTextViewAsync()
        {
            var textManager = await _visualStudioServices.GetTextManagerServiceAsync();
            textManager.GetActiveView(1, null, out var textView);
            var editorAdaptersFactoryService = await _visualStudioServices.GetEditorAdaptersFactoryServiceAsync();
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

        private static TextSpan GetSelection(ITextView activeWpfTextView)
        {
            var visualStudioSpan = activeWpfTextView.Selection.StreamSelectionSpan.SnapshotSpan.Span;
            var roslynSpan = new TextSpan(visualStudioSpan.Start, visualStudioSpan.Length);
            return roslynSpan;
        }
    }
}