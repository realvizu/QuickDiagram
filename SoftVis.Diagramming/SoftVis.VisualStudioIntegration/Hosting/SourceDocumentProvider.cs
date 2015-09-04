using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    public class SourceDocumentProvider : ISourceDocumentProvider, IVsRunningDocTableEvents, IDisposable
    {
        private const string CSHARP_CONTENTTYPE_NAME = "CSharp";

        private readonly IHostServiceProvider _hostServiceProvider;
        private uint _runningDocumentTableCookie;
        private IVsRunningDocumentTable _runningDocumentTable;

        public IWpfTextView ActiveWpfTextView { get; private set; }

        public SourceDocumentProvider(IHostServiceProvider hostServiceProvider)
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

                ActiveWpfTextView = contentType.IsOfType(CSHARP_CONTENTTYPE_NAME)
                    ? wpfTextView
                    : null;
            }

            return VSConstants.S_OK;
        }

        public Document GetCurrentDocument()
        {
            if (ActiveWpfTextView == null)
                return null;

            var currentSnapshot = ActiveWpfTextView.TextBuffer.CurrentSnapshot;
            var contentType = currentSnapshot.ContentType;
            if (!contentType.IsOfType("CSharp"))
                return null;

            var document = currentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            return document;
        }

        public TextSpan GetSelection()
        {
            var visualStudioSpan = ActiveWpfTextView.Selection.StreamSelectionSpan.SnapshotSpan.Span;
            var roslynSpan = new TextSpan(visualStudioSpan.Start, visualStudioSpan.Length);
            return roslynSpan;
        }

        public Workspace GetWorkspace()
        {
            if (ActiveWpfTextView == null)
                return null;

            if (ActiveWpfTextView.TextBuffer == null)
                return null;

            return ActiveWpfTextView.TextBuffer.GetWorkspace();
        }

        private IWpfTextView VsWindowFrameToWpfTextView(IVsWindowFrame vsWindowFrame)
        {
            IWpfTextView wpfTextView = null;
            var textView = VsShellUtilities.GetTextView(vsWindowFrame);
            if (textView != null)
            {
                var riidKey = DefGuidList.guidIWpfTextViewHost;
                object pvtData;
                var vsUserData = (Microsoft.VisualStudio.TextManager.Interop.IVsUserData)textView;
                if (vsUserData.GetData(ref riidKey, out pvtData) == 0 && pvtData != null)
                    wpfTextView = ((IWpfTextViewHost)pvtData).TextView;
            }
            return wpfTextView;
        }

        private void InitializeRunningDocumentTable()
        {
            if (RunningDocumentTable == null)
                return;

            RunningDocumentTable.AdviseRunningDocTableEvents(this, out _runningDocumentTableCookie);
        }

        private IVsRunningDocumentTable RunningDocumentTable
        {
            get
            {
                if (_runningDocumentTable == null)
                    _runningDocumentTable = _hostServiceProvider.GetService<IVsRunningDocumentTable, SVsRunningDocumentTable>();
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
