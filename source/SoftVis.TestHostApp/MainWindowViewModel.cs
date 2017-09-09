using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Service;
using Codartis.SoftVis.Service.Plugins;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;
using Codartis.SoftVis.TestHostApp.UI;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;
using Codartis.SoftVis.Util.UI.Wpf.Resources;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

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

        public MainWindowViewModel()
        {
            SelectedDpi = 300;

            var visualizationService = new VisualizationService(
                new TestModelServiceFactory(),
                new TestDiagramServiceFactory(),
                new TestUiServiceFactory(),
                new DiagramPluginFactory(new TestLayoutPriorityProvider(), new TestDiagramShapeFactory()),
                new[]
                {
                    DiagramPluginId.AutoLayoutDiagramPlugin,
                    DiagramPluginId.ConnectorHandlerDiagramPlugin,
                    DiagramPluginId.ModelTrackingDiagramPlugin
                }
            );

            _testModelService = (ITestModelService)visualizationService.GetModelService();
            var diagramId = visualizationService.CreateDiagram(minZoom: 0.2, maxZoom: 5, initialZoom: 1);
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

        public void OnUiInitialized(Window mainWindow, IDiagramStlyeProvider diagramStlyeProvider)
        {
            _window = mainWindow;

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            _uiService.Initialize(resourceDictionary, diagramStlyeProvider);
        }

        private void OnShowModelItemsRequested(IReadOnlyList<IModelNode> modelNodes, bool followNewDiagramNodes)
        {
            var diagramNodes = _diagramService.ShowModelNodes(modelNodes);

            if (followNewDiagramNodes)
                _uiService.FollowDiagramNodes(diagramNodes);
        }

        private void AddShapes()
        {
            var model = _testModelService.TestModel;

            if (_modelItemGroupIndex == model.ItemGroups.Count)
                return;

            var modelNodes = model.ItemGroups[_modelItemGroupIndex];

            foreach (var modelNode in modelNodes)
                _diagramService.ShowModelNode(modelNode);

            _modelItemGroupIndex++;

            //_testDiagram.Save(@"c:\big.xml");

            ZoomToContent();
        }

        private void RemoveShapes()
        {
            var model = _testModelService.TestModel;

            if (_nextToRemoveModelItemGroupIndex == model.ItemGroups.Count)
                return;

            var modelNodes = model.ItemGroups[_nextToRemoveModelItemGroupIndex];

            foreach (var modelNode in modelNodes)
                _diagramService.HideModelNode(modelNode);

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
                    progressDialog.ShowWithDelayAsync();

                    var bitmapSource = await _uiService.CreateDiagramImageAsync(SelectedDpi, 10,
                        progressDialog.CancellationToken, progressDialog.Progress, progressDialog.MaxProgress);

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
