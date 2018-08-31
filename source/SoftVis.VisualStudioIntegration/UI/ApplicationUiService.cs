using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;
using Codartis.SoftVis.Util.UI.Wpf.Resources;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Provides diagram UI services. Bundles the diagram control and its view model together.
    /// </summary>
    internal sealed class ApplicationUiService : WpfUiService, IApplicationUiService
    {
        private const string DialogTitle = "Quick Diagram Tool";
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";
        private const double ExportedImageMargin = 10;

        private readonly IHostUiServices _hostUiServices;
        private readonly RoslynDiagramViewModel _diagramViewModel;

        public DiagramControl DiagramControl { get; }
        public Dpi ImageExportDpi { get; set; }

        public ApplicationUiService(IHostUiServices hostUiServices, RoslynDiagramViewModel diagramViewModel)
            : base(diagramViewModel)
        {
            _hostUiServices = hostUiServices;
            _diagramViewModel = diagramViewModel;

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());
            DiagramControl = new DiagramControl(resourceDictionary) {DataContext = _diagramViewModel};

            Initialize(resourceDictionary, DiagramControl);
        }

        public void ShowDiagramWindow() => _hostUiServices.ShowDiagramWindow();

        public void ShowMessageBox(string message)
            => System.Windows.MessageBox.Show(message, DialogTitle);

        public void ShowPopupMessage(string message, TimeSpan hideAfter = default(TimeSpan))
            => _diagramViewModel.ShowPopupMessage(message, hideAfter);

        public string SelectSaveFilename(string title, string filter)
        {
            var saveFileDialog = new SaveFileDialog {Title = title, Filter = filter};
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }

        public void ExpandAllNodes() => _diagramViewModel.ExpandAllNodes();
        public void CollapseAllNodes() => _diagramViewModel.CollapseAllNodes();

        public async Task<ProgressDialog> CreateProgressDialogAsync(string text, int maxProgress = 0)
        {
            var hostMainWindow = await _hostUiServices.GetMainWindowAsync();
            return new ProgressDialog(hostMainWindow, DialogTitle, text, maxProgress);
        }

        public async Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null)
        {
            try
            {
                return await CreateDiagramImageAsync(ImageExportDpi.Value, ExportedImageMargin,
                    cancellationToken, progress, maxProgress);
            }
            catch (OutOfMemoryException)
            {
                HandleOutOfMemory();
                return null;
            }
        }

        private void HandleOutOfMemory()
        {
            ShowMessageBox("Cannot generate the image because it is too large. Please select a smaller DPI value.");
        }
    }
}