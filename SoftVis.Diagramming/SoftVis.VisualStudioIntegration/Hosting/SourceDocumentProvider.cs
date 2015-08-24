using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    public class SourceDocumentProvider : ISourceDocumentProvider, IVsRunningDocTableEvents, IDisposable
    {
        private const string CSHARP_CONTENTTYPE_NAME = "CSharp";

        private readonly SoftVisPackage _package;
        private uint _runningDocumentTableCookie;
        private IVsRunningDocumentTable _runningDocumentTable;

        public IWpfTextView ActiveWpfTextView { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public SourceDocumentProvider(SoftVisPackage package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            _package = package;

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
            IWpfTextView wpfTextView = VsWindowFrameToWpfTextView(pFrame);
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
            var span = ActiveWpfTextView.Selection.StreamSelectionSpan.SnapshotSpan.Span;
            Debug.Assert(span != null);
            return new TextSpan(span.Start, span.Length);
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
                    _runningDocumentTable = GetService<IVsRunningDocumentTable, SVsRunningDocumentTable>(SoftVisPackage.GlobalServiceProvider);
                return _runningDocumentTable;
            }
        }

        void IDisposable.Dispose()
        {
            if ((int)_runningDocumentTableCookie == 0)
                return;
            _runningDocumentTable.UnadviseRunningDocTableEvents(this._runningDocumentTableCookie);
            _runningDocumentTableCookie = 0U;
        }

        private static TServiceInterface GetService<TServiceInterface, TService>(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider) where TServiceInterface : class where TService : class
        {
            return (TServiceInterface)GetService(serviceProvider, typeof(TService).GUID, false);
        }

        private static object GetService(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, Guid guidService, bool unique)
        {
            var riid = VSConstants.IID_IUnknown;
            var ppvObject = IntPtr.Zero;
            object obj = null;
            if (serviceProvider.QueryService(ref guidService, ref riid, out ppvObject) == 0)
            {
                if (ppvObject != IntPtr.Zero)
                {
                    try
                    {
                        obj = !unique 
                            ? Marshal.GetObjectForIUnknown(ppvObject) 
                            : Marshal.GetUniqueObjectForIUnknown(ppvObject);
                    }
                    finally
                    {
                        Marshal.Release(ppvObject);
                    }
                }
            }
            return obj;
        }
    }
}
