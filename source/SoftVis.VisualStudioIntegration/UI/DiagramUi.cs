using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util;
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

        private readonly ResourceDictionary _resourceDictionary;
        private readonly DiagramViewModel _diagramViewModel;
        public DiagramControl DiagramControl { get; }

        public Dpi ImageExportDpi { get; set; }

        public event Action<IDiagramShape> ShowSourceRequested;
        public event Action<IReadOnlyList<IModelEntity>> ShowModelItemsRequested;

        public DiagramUi(IArrangedDiagram diagram)
        {
            _resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            _diagramViewModel = new DiagramViewModel(diagram, minZoom: .1, maxZoom: 10, initialZoom: 1);
            DiagramControl = new DiagramControl(_resourceDictionary) { DataContext = _diagramViewModel };

            SubscribeToDiagramViewModelEvents(_diagramViewModel);
        }

        public void ShowMessageBox(string message) => System.Windows.MessageBox.Show(message, DialogTitle);

        public void ShowPopupMessage(string message, TimeSpan hideAfter = default) => _diagramViewModel.ShowPopupMessage(message, hideAfter);

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

        public async Task<BitmapSource> CreateDiagramImageAsync(
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null,
            IProgress<int> maxProgress = null)
        {
            try
            {
                // The image creator must be created on the UI thread so it can read the necessary view and view model data.
                var diagramImageCreator = new DataCloningDiagramImageCreator(_diagramViewModel, DiagramControl, _resourceDictionary);

                return await Task.Factory.StartSTA(
                    () =>
                        diagramImageCreator.CreateImage(ImageExportDpi.Value, ExportedImageMargin, cancellationToken, progress, maxProgress),
                    cancellationToken);
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
            diagramViewModel.ShowModelItemsRequested += i => ShowModelItemsRequested?.Invoke(i);
        }
    }
}