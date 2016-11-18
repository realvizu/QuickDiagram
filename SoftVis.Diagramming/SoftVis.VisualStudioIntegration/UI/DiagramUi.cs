using System;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Provides diagram UI services. Bundles the diagram control and its view model together.
    /// </summary>
    public sealed class DiagramUi : IUiServices
    {
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";
        private const double ExportedImageMargin = 10;

        private readonly DiagramControl _diagramControl;
        private readonly DiagramViewModel _diagramViewModel;

        public Dpi ImageExportDpi { get; set; }

        public DiagramUi(IArrangedDiagram diagram)
        {
            ImageExportDpi = Dpi.Default;

            _diagramViewModel = new DiagramViewModel(diagram, minZoom: .1, maxZoom: 10, initialZoom: 1);

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());
            _diagramControl = new DiagramControl(resourceDictionary) { DataContext = _diagramViewModel };
        }

        public object ContentControl => _diagramControl;

        public void FitDiagramToView()
            => _diagramViewModel.ZoomToContent();

        public void GetDiagramImage(Action<BitmapSource> imageCreatedCallback)
            => _diagramViewModel.GetDiagramImage(ImageExportDpi.Value, ExportedImageMargin, imageCreatedCallback);

        public void MessageBox(string message)
        {
            System.Windows.MessageBox.Show(message, "Diagram Tool");
        }

        public string SelectSaveFilename(string title, string filter)
        {
            var saveFileDialog1 = new SaveFileDialog { Title = title, Filter = filter };
            saveFileDialog1.ShowDialog();
            return saveFileDialog1.FileName;
        }
    }
}
