using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Services;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util;
using Codartis.Util.UI.Wpf.Commands;
using Codartis.Util.UI.Wpf.Dialogs;
using Codartis.Util.UI.Wpf.Resources;
using Codartis.Util.UI.Wpf.ViewModels;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private const string DiagramStylesXaml = "Resources/Styles.xaml";

        private readonly ITestModelService _testModelService;
        private readonly IDiagramService _diagramService;
        private readonly IWpfUiService _uiService;

        private int _modelItemGroupIndex;
        private int _nextToRemoveModelItemGroupIndex;
        private double _selectedDpi;
        private Window _window;

        public DiagramViewModel DiagramViewModel => _uiService.DiagramViewModel;
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ZoomToContentCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        public MainWindowViewModel([NotNull] IVisualizationService visualizationService)
        {
            SelectedDpi = 300;

            var modelService = visualizationService.GetModelService();
            _testModelService = new TestModelService(modelService);
            var diagramId = visualizationService.CreateDiagram();
            _diagramService = visualizationService.GetDiagramService(diagramId);
            _uiService = (IWpfUiService)visualizationService.GetUiService(diagramId);

            _uiService.DiagramNodeInvoked += i => Debug.WriteLine($"DiagramNodeInvoked: {i}");
            _uiService.ShowModelItemsRequested += OnShowModelItemsRequested;

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

        public void OnUiInitialized(Window mainWindow, IDiagramStyleProvider diagramStyleProvider)
        {
            _window = mainWindow;

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            _uiService.Initialize(resourceDictionary, diagramStyleProvider);
        }

        private void OnShowModelItemsRequested(IReadOnlyList<IModelNode> modelNodes, bool followNewDiagramNodes)
        {
            _diagramService.AddNodes(modelNodes.Select(i => i.Id));

            if (followNewDiagramNodes)
                _uiService.FollowDiagramNodes(modelNodes.Select(i => i.Id).ToArray());
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
                _uiService.ZoomToDiagram();
            };
            timer.Start();
        }

        private async void CopyToClipboardAsync()
        {
            try
            {
                using (var progressDialog = new ProgressDialog(_window, "TestHostApp", "Generating image.."))
                {
                    await progressDialog.ShowWithDelayAsync();

                    var bitmapSource = await _uiService.CreateDiagramImageAsync(
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