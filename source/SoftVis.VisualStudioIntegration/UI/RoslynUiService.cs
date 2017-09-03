using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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
    internal sealed class RoslynUiService : WpfUiService, IRoslynUiService
    {
        private const string DialogTitle = "Quick Diagram Tool";
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";
        private const double ExportedImageMargin = 10;

        private readonly IHostUiServices _hostUiServices;
        private readonly RoslynDiagramViewModel _diagramViewModel;
        private readonly ResourceDictionary _resourceDictionary;
        private readonly DiagramControl _diagramControl;

        public Dpi ImageExportDpi { get; set; }

        public RoslynUiService(IHostUiServices hostUiServices, RoslynDiagramViewModel diagramViewModel)
            : base(diagramViewModel)
        {
            _hostUiServices = hostUiServices;
            _diagramViewModel = diagramViewModel;

            _resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());
            _diagramControl = new DiagramControl(_resourceDictionary) { DataContext = _diagramViewModel };
            Initialize(_resourceDictionary, _diagramControl);

            hostUiServices.HostDiagram(_diagramControl);
        }

        public void ShowDiagramWindow() => _hostUiServices.ShowDiagramWindow();

        public void ShowMessageBox(string message)
            => System.Windows.MessageBox.Show(message, DialogTitle);

        public void ShowPopupMessage(string message, TimeSpan hideAfter = default(TimeSpan))
            => _diagramViewModel.ShowPopupMessage(message, hideAfter);

        public string SelectSaveFilename(string title, string filter)
        {
            var saveFileDialog = new SaveFileDialog { Title = title, Filter = filter };
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }

        public void ExpandAllNodes() => _diagramViewModel.ExpandAllNodes();
        public void CollapseAllNodes() => _diagramViewModel.CollapseAllNodes();

        public void ExecuteWhenUiIsIdle(Action action)
            => Dispatcher.CurrentDispatcher.BeginInvoke(action, DispatcherPriority.Background);

        public ProgressDialog CreateProgressDialog(string text, int maxProgress = 0)
        {
            var hostMainWindow = _hostUiServices.GetMainWindow();
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
