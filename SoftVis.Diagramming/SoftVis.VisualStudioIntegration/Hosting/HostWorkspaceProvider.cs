using System;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.VisualStudioIntegration.App;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Document = Microsoft.CodeAnalysis.Document;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Gets information from Visual Studio about the current solution, projects, source documents.
    /// </summary>
    internal class HostWorkspaceProvider : IHostWorkspaceServices, IVsRunningDocTableEvents, IDisposable
    {
        private const string CSharpContentTypeName = "CSharp";

        private readonly IPackageServices _packageServices;
        private uint _runningDocumentTableCookie;
        private IVsRunningDocumentTable _runningDocumentTable;
        private IWpfTextView _activeWpfTextView;

        internal HostWorkspaceProvider(IPackageServices packageServices)
        {
            _packageServices = packageServices;
            InitializeRunningDocumentTable();
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            var wpfTextView = VsWindowFrameToWpfTextView(pFrame);
            if (wpfTextView != null)
            {
                var contentType = wpfTextView.TextBuffer.ContentType;

                _activeWpfTextView = contentType.IsOfType(CSharpContentTypeName)
                    ? wpfTextView
                    : null;
            }

            return VSConstants.S_OK;
        }

        public Workspace GetWorkspace()
        {
            return _packageServices.GetVisualStudioWorkspace();
        }

        public async Task<ISymbol> GetCurrentSymbol()
        {
            var document = GetCurrentDocument();
            if (document == null)
                return null;

            var syntaxTree = await document.GetSyntaxTreeAsync();
            var syntaxRoot = syntaxTree.GetRoot();
            var span = GetSelection();
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

        public void ShowSourceFile(ISymbol symbol)
        {
            var workspace = GetWorkspace();

            var location = symbol?.Locations.FirstOrDefault();
            if (location == null)
                return;

            var documentId = workspace?.CurrentSolution?.GetDocumentId(location?.SourceTree);
            if (documentId == null)
                return;

            workspace.OpenDocument(documentId);

            var hostService = _packageServices.GetHostService2();
            var selection = hostService.ActiveDocument.Selection as TextSelection;
            if (selection == null)
                return;

            var fileLinePositionSpan = location.GetLineSpan();
            var start = fileLinePositionSpan.Span.Start;
            selection.MoveTo(start.Line + 1, start.Character + 1, false);
            var end = fileLinePositionSpan.Span.End;
            selection.MoveTo(end.Line + 1, end.Character + 1, true);
        }

        private Document GetCurrentDocument()
        {
            if (_activeWpfTextView == null)
                return null;

            var currentSnapshot = _activeWpfTextView.TextBuffer.CurrentSnapshot;
            var contentType = currentSnapshot.ContentType;
            if (!contentType.IsOfType(CSharpContentTypeName))
                return null;

            var document = currentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            return document;
        }

        private TextSpan GetSelection()
        {
            var visualStudioSpan = _activeWpfTextView.Selection.StreamSelectionSpan.SnapshotSpan.Span;
            var roslynSpan = new TextSpan(visualStudioSpan.Start, visualStudioSpan.Length);
            return roslynSpan;
        }

        private static IWpfTextView VsWindowFrameToWpfTextView(IVsWindowFrame vsWindowFrame)
        {
            IWpfTextView wpfTextView = null;
            var textView = VsShellUtilities.GetTextView(vsWindowFrame);
            if (textView != null)
            {
                var riidKey = DefGuidList.guidIWpfTextViewHost;
                object pvtData;
                var vsUserData = (IVsUserData)textView;
                if (vsUserData.GetData(ref riidKey, out pvtData) == 0 && pvtData != null)
                    wpfTextView = ((IWpfTextViewHost)pvtData).TextView;
            }
            return wpfTextView;
        }

        private void InitializeRunningDocumentTable()
        {
            RunningDocumentTable?.AdviseRunningDocTableEvents(this, out _runningDocumentTableCookie);
        }

        private IVsRunningDocumentTable RunningDocumentTable
        {
            get
            {
                if (_runningDocumentTable == null)
                    _runningDocumentTable = _packageServices.GetRunningDocumentTableService();
                return _runningDocumentTable;
            }
        }

        void IDisposable.Dispose()
        {
            if ((int)_runningDocumentTableCookie == 0)
                return;
            _runningDocumentTable.UnadviseRunningDocTableEvents(_runningDocumentTableCookie);
            _runningDocumentTableCookie = 0U;
        }
    }
}
