using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Rendering.Wpf;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Codartis.SoftVis.VisualStudioIntegration.Presentation
{
    /// <summary>
    /// Implements a Visual Studio tool window that displays a diagram.
    /// </summary>
    [Guid("02d1f8b9-d0a0-4ccb-9687-e6f0f781ad9e")]
    public class DiagramToolWindow : ToolWindowPane, IDiagramWindow
    {
        private readonly DiagramViewerControl _diagramViewerControl;
        private ISourceDocumentProvider _sourceDocumentProvider;
        private RoslynBasedWpfDiagramBuilder _diagramBuilder;

        public Dpi ImageExportDpi { get; set; }

        public DiagramToolWindow() : base(null)
        {
            _diagramViewerControl = new DiagramViewerControl();

            Caption = "Diagram";
            ToolBar = new CommandID(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ToolWindowToolbar);
            Content = _diagramViewerControl;
            ImageExportDpi = Dpi.Default;
        }

        internal void Initialize(ISourceDocumentProvider sourceDocumentProvider)
        {
            _sourceDocumentProvider = sourceDocumentProvider;
            _diagramBuilder = new RoslynBasedWpfDiagramBuilder(_sourceDocumentProvider);
            _diagramViewerControl.Diagram = _diagramBuilder.Diagram;
        }

        public int FontSize
        {
            get { return (int)_diagramViewerControl.FontSize; }
            set { _diagramViewerControl.FontSize = value; }
        }

        public void Show()
        {
            var windowFrame = (IVsWindowFrame)Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        public void AddCurrentSymbol()
        {
            _diagramBuilder.AddCurrentSymbol();
            _diagramViewerControl.FitDiagramToView();
        }

        public void Clear()
        {
            _diagramBuilder.Clear();
        }

        public BitmapSource GetDiagramAsBitmap()
        {
            return _diagramViewerControl.GetDiagramAsBitmap(ImageExportDpi.Value);
        }
    }
}
