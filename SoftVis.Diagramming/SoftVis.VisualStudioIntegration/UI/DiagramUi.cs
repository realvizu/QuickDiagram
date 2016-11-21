using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Controls;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;
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
        private readonly ThreadIndependentDiagramImageCreator _diagramImageCreator;
        private readonly ProgressWindow _progressWindow;
        private readonly ProgressWindowViewModel _progressViewModel;

        public Dpi ImageExportDpi { get; set; }

        public DiagramUi(IArrangedDiagram diagram)
        {
            ImageExportDpi = Dpi.Default;

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            _diagramViewModel = new DiagramViewModel(diagram, minZoom: .1, maxZoom: 10, initialZoom: 1);
            _diagramControl = new DiagramControl(resourceDictionary) { DataContext = _diagramViewModel };
            _diagramImageCreator = new ThreadIndependentDiagramImageCreator(_diagramViewModel, _diagramControl, resourceDictionary);

            _progressViewModel = new ProgressWindowViewModel();
            _progressWindow = new ProgressWindow {DataContext = _progressViewModel};
            // TODO: set Owner
        }

        public object ContentControl => _diagramControl;

        public void FitDiagramToView() => _diagramViewModel.ZoomToContent();

        public void GetDiagramImage(Action<BitmapSource> imageCreatedCallback)
        {
            ShowProgressWindow("Generating diagram image...", null);

            try
            {
                //_diagramViewModel.GetDiagramImage(ImageExportDpi.Value, ExportedImageMargin,
                //    imageCreatedCallback, SetProgress);
            }
            catch (OutOfMemoryException)
            {
                MessageBox("Cannot export the image because it is too large. Please select a smaller DPI value.");
            }
            finally
            {
                HideProgressWindow();
            }
        }

        public void MessageBox(string message)
        {
            System.Windows.MessageBox.Show(message, "Diagram Tool");
        }

        public void ShowProgressWindow(string text, Action cancelAction, double progress = 0)
        {
            _progressViewModel.Title = "Diagram Tool";
            _progressViewModel.Text = text;
            _progressViewModel.ProgressValue = progress;
            _progressWindow.Show();
        }

        private void OnProgressWindowCanceled(Action cancelAction)
        {
            HideProgressWindow();
            cancelAction?.Invoke();
        }

        public void SetProgress(double progress)
        {
            _progressViewModel.ProgressValue = progress;
        }

        public void HideProgressWindow()
        {
            _progressWindow.Hide();
        }

        public string SelectSaveFilename(string title, string filter)
        {
            var saveFileDialog1 = new SaveFileDialog { Title = title, Filter = filter };
            saveFileDialog1.ShowDialog();
            return saveFileDialog1.FileName;
        }
    }
}
