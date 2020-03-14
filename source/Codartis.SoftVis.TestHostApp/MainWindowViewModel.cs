using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;
using Codartis.SoftVis.UI.Wpf;
using Codartis.Util;
using Codartis.Util.UI.Wpf.Commands;
using Codartis.Util.UI.Wpf.Dialogs;
using Codartis.Util.UI.Wpf.ViewModels;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp
{
    internal class MainWindowViewModel : ViewModelBase
    {
        [NotNull] private readonly Window _window;
        [NotNull] private readonly ITestModelService _testModelService;
        [NotNull] private readonly IDiagramService _diagramService;
        [NotNull] private readonly IWpfDiagramUiService _wpfDiagramUiService;

        private int _modelItemGroupIndex;
        private int _nextToRemoveModelItemGroupIndex;
        private double _selectedDpi;

        [NotNull] public ICommand AddCommand { get; }
        [NotNull] public ICommand RemoveCommand { get; }
        [NotNull] public ICommand ZoomToContentCommand { get; }
        [NotNull] public ICommand CopyToClipboardCommand { get; }

        public MainWindowViewModel(
            [NotNull] Window mainWindow,
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService,
            [NotNull] IWpfDiagramUiService wpfDiagramUiService)
        {
            SelectedDpi = 300;

            _window = mainWindow;

            _testModelService = new TestModelService(modelService);

            _diagramService = diagramService;

            _wpfDiagramUiService = wpfDiagramUiService;
            _wpfDiagramUiService.DiagramNodeInvoked += i => Debug.WriteLine($"DiagramNodeInvoked: {i}");
            _wpfDiagramUiService.ShowModelItemsRequested += OnShowModelItemsRequested;

            AddCommand = new DelegateCommand(AddShapes);
            RemoveCommand = new DelegateCommand(RemoveShapes);
            ZoomToContentCommand = new DelegateCommand(ZoomToContent);
            CopyToClipboardCommand = new DelegateCommand(CopyToClipboardAsync);

            PopulateModel(_testModelService);
        }

        public double SelectedDpi
        {
            get { return _selectedDpi; }
            set
            {
                _selectedDpi = value;
                OnPropertyChanged();
            }
        }

        private void OnShowModelItemsRequested(IReadOnlyList<ModelNodeId> modelNodeIds, bool followNewDiagramNodes)
        {
            _diagramService.AddNodes(modelNodeIds);

            if (followNewDiagramNodes)
                _wpfDiagramUiService.FollowDiagramNodes(modelNodeIds);
        }

        private void AddShapes()
        {
            if (_modelItemGroupIndex == _testModelService.ItemGroups.Count)
                return;

            var modelNodes = _testModelService.ItemGroups[_modelItemGroupIndex];

            _diagramService.AddNodes(modelNodes.Select(i => i.Id));

            _modelItemGroupIndex++;

            //_testDiagram.Save(@"c:\big.xml");

            ZoomToContent();
        }

        private void RemoveShapes()
        {
            if (_nextToRemoveModelItemGroupIndex == _testModelService.ItemGroups.Count)
                return;

            var modelNodes = _testModelService.ItemGroups[_nextToRemoveModelItemGroupIndex];

            foreach (var modelNode in modelNodes)
                _diagramService.RemoveNode(modelNode.Id);

            _nextToRemoveModelItemGroupIndex++;

            ZoomToContent();
        }

        private void ZoomToContent()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += (s, o) =>
            {
                timer.Stop();
                _wpfDiagramUiService.ZoomToDiagram();
            };
            timer.Start();
        }

        private async void CopyToClipboardAsync()
        {
            try
            {
                using (var progressDialog = new ProgressDialog(_window, "TestHostApp", "Generating image.."))
                {
                    progressDialog.ShowWithDelay();

                    var bitmapSource = await _wpfDiagramUiService.CreateDiagramImageAsync(
                        SelectedDpi,
                        10,
                        progressDialog.CancellationToken,
                        progressDialog.Progress,
                        progressDialog.MaxProgress);

                    progressDialog.Reset("Copying image to clipboard...", showProgressNumber: false);

                    // Clipboard operations must run on STA thread.
                    await Task.Factory.StartSTA(() => Clipboard.SetImage(bitmapSource), progressDialog.CancellationToken);
                }
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Cannot export the image because it is too large. Please select a smaller DPI value.", "TestHostApp");
            }
        }

        private static void PopulateModel(ITestModelService testModelService)
        {
            TestModelCreator.Create(testModelService);
            //BigTestModelCreator.Create(_testModelService, 2, 5);
        }
    }
}