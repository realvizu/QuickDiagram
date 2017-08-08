using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;
using Codartis.SoftVis.Util.UI.Wpf.Resources;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Provides diagram UI services. Bundles the diagram control and its view model together.
    /// </summary>
    public sealed class DiagramUi : IUiServices
    {
        private const string DialogTitle = "Quick Diagram Tool";
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";
        private const double ExportedImageMargin = 10;

        private readonly IHostUiServices _hostUiServices;
        private readonly ResourceDictionary _resourceDictionary;
        private readonly DiagramViewModel _diagramViewModel;
        private readonly DiagramControl _diagramControl;

        public Dpi ImageExportDpi { get; set; }

        public event Action<IDiagramShape> ShowSourceRequested;
        //public event Action<IReadOnlyList<IModelEntity>> ShowModelItemsRequested;

        public DiagramUi(IHostUiServices hostUiServices, IArrangedDiagram diagram)
        {
            _hostUiServices = hostUiServices;
            _resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            var diagramShapeViewModelFactory = new RoslynDiagramShapeViewModelFactory(diagram);
            _diagramViewModel = new DiagramViewModel(diagram, diagramShapeViewModelFactory, minZoom: .1, maxZoom: 10, initialZoom: 1);
            _diagramControl = new DiagramControl(_resourceDictionary) { DataContext = _diagramViewModel };

            hostUiServices.HostDiagram(_diagramControl);

            SubscribeToDiagramViewModelEvents(_diagramViewModel);
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

        public void FollowDiagramNode(IDiagramNode diagramNode) => _diagramViewModel.FollowDiagramNodes(new[] { diagramNode });
        public void FollowDiagramNodes(IReadOnlyList<IDiagramNode> diagramNodes) => _diagramViewModel.FollowDiagramNodes(diagramNodes);
        public void ZoomToDiagram() => _diagramViewModel.ZoomToContent();
        public void KeepDiagramCentered() => _diagramViewModel.KeepDiagramCentered();
        public void ExpandAllNodes() => _diagramViewModel.ExpandAllNodes();
        public void CollapseAllNodes() => _diagramViewModel.CollapseAllNodes();

        public void ZoomToDiagramNode(IDiagramNode diagramNode)
        {
            var rect = diagramNode.Rect.ToWpf();
            if (rect.IsDefined())
                _diagramViewModel.EnsureRectIsVisible(rect);
        }

        public void ZoomToDiagramNodes(IEnumerable<IDiagramNode> diagramNodes)
        {
            var rect = diagramNodes.Select(i => i.Rect).Where(i => i.IsDefined()).Union().ToWpf();
            if (rect.IsDefined())
                _diagramViewModel.ZoomToRect(rect);
        }

        public void EnsureDiagramNodeIsVisible(IDiagramNode diagramNode)
        {
            var rect = diagramNode.Rect.ToWpf();
            if (rect.IsDefined())
                _diagramViewModel.EnsureRectIsVisible(rect);
        }

        public void EnsureDiagramIsVisible()
        {
            if (!_diagramViewModel.IsDiagramContentVisible())
                _diagramViewModel.ZoomToContent();
        }

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
                // The image creator must be created on the UI thread so it can read the necessary view and view model data.
                var diagramImageCreator = new DataCloningDiagramImageCreator(_diagramViewModel, _diagramControl, _resourceDictionary);

                return await Task.Factory.StartSTA(() =>
                    diagramImageCreator.CreateImage(ImageExportDpi.Value, ExportedImageMargin, cancellationToken, progress, maxProgress), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
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

        private void SubscribeToDiagramViewModelEvents(DiagramViewModel diagramViewModel)
        {
            diagramViewModel.ShowSourceRequested += i => ShowSourceRequested?.Invoke(i);
            //diagramViewModel.ShowModelItemsRequested += i => ShowModelItemsRequested?.Invoke(i);
        }
    }
}
