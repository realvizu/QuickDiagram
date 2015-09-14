using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Codartis.SoftVis.VisualStudioIntegration.WorkspaceContext
{
    /// <summary>
    /// Gets information from Visual Studio about the currently edited source document.
    /// </summary>
    internal class WorkspaceServices : IWorkspaceServices, IVsRunningDocTableEvents, IDisposable
    {
        private const string CSharpContentTypeName = "CSharp";

        private readonly IHostServiceProvider _hostServiceProvider;
        private uint _runningDocumentTableCookie;
        private IVsRunningDocumentTable _runningDocumentTable;
        private IWpfTextView _activeWpfTextView;

        internal WorkspaceServices(IHostServiceProvider hostServiceProvider)
        {
            _hostServiceProvider = hostServiceProvider;
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
            return _hostServiceProvider.GetVisualStudioWorkspace();
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
            var symbolInfo = semanticModel.GetSymbolInfo(currentNode);
            var symbol = symbolInfo.Symbol ?? semanticModel.GetDeclaredSymbol(currentNode);
            return symbol;
        }

        public void ShowSourceFile(ISymbol symbol)
        {
            var workspace = GetWorkspace();
            var documentId = workspace?.CurrentSolution?.GetDocumentId(symbol?.Locations.FirstOrDefault()?.SourceTree);

            if (documentId != null)
                workspace.OpenDocument(documentId);
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
                    _runningDocumentTable = _hostServiceProvider.GetRunningDocumentTableService();
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
