using Codartis.SoftVis.Rendering.Wpf;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.Presentation
{
    [Guid("02d1f8b9-d0a0-4ccb-9687-e6f0f781ad9e")]
    public class DiagramToolWindow : ToolWindowPane, IDiagramWindow
    {
        private const double MinFontSize = 6;
        private const double MaxFontSize = 24;

        private readonly DiagramViewerControl _diagramViewerControl;
        private ISourceDocumentProvider _sourceDocumentProvider;
        private DiagramBuilder _diagramBuilder;

        public DiagramToolWindow() : base(null)
        {
            _diagramViewerControl = new DiagramViewerControl();

            Content = _diagramViewerControl;
            Caption = "Diagram";
            ToolBar = new CommandID(VsctConstants.ToolWindowToolbarCommands, VsctConstants.ToolWindowToolbar);
        }

        internal void Initialize(ISourceDocumentProvider sourceDocumentProvider)
        {
            _sourceDocumentProvider = sourceDocumentProvider;
            _diagramBuilder = new DiagramBuilder(_sourceDocumentProvider);
            _diagramViewerControl.Diagram = _diagramBuilder.Diagram;
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

        public void IncreaseFontSize()
        {
            _diagramViewerControl.FontSize = Math.Min(_diagramViewerControl.FontSize + 1, MaxFontSize);
        }

        public void DecreaseFontSize()
        {
            _diagramViewerControl.FontSize = Math.Max(_diagramViewerControl.FontSize - 1, MinFontSize);
        }

        public BitmapSource GetDiagramAsBitmap()
        {
            return _diagramViewerControl.GetDiagramAsBitmap();
        }
    }
}
