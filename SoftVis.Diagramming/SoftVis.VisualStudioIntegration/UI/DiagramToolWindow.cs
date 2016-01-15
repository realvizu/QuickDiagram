using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Implements a Visual Studio tool window that displays a diagram.
    /// </summary>
    [Guid("02d1f8b9-d0a0-4ccb-9687-e6f0f781ad9e")]
    public sealed class DiagramToolWindow : ToolWindowPane
    {
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";

        private readonly DiagramControl _diagramControl;

        public Dpi ImageExportDpi { get; set; }

        public DiagramToolWindow() : base(null)
        {
            var resourceDictionary = WpfHelpers.GetResourceDictionary(DiagramStylesXaml);
            _diagramControl = new DiagramControl(resourceDictionary);

            Caption = "Diagram";
            ToolBar = new CommandID(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ToolWindowToolbar);
            Content = _diagramControl;
            ImageExportDpi = Dpi.Default;
        }

        internal void Initialize(IModel model, Diagram diagram)
        {
            var diagramBehaviourProvider = new CustomDiagramBehaviourProvider();
            _diagramControl.DataContext = new DiagramViewerViewModel(model, diagram, diagramBehaviourProvider);
        }

        public int FontSize
        {
            get { return (int)_diagramControl.FontSize; }
            set { _diagramControl.FontSize = value; }
        }

        public void Show()
        {
            var windowFrame = (IVsWindowFrame)Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        public void FitDiagramToView()
        {
            _diagramControl.FitToContent();
        }

        public BitmapSource GetDiagramAsBitmap()
        {
            return null; // _diagramControl.GetDiagramAsBitmap(ImageExportDpi.Value);
        }
    }
}
