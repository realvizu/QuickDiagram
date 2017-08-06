using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;
using TestModelBuilder = Codartis.SoftVis.TestHostApp.Modeling.TestModelBuilder;

namespace Codartis.SoftVis.TestHostApp
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly TestModelBuilder _testModelBuilder;
        private readonly Diagramming.TestDiagram _testDiagram;

        private int _modelItemGroupIndex;
        private int _nextToRemoveModelItemGroupIndex;
        private double _selectedDpi;

        public DiagramViewModel DiagramViewModel { get; }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ZoomToContentCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        public MainWindow Window { get; set; }
        public IDiagramStlyeProvider DiagramStlyeProvider { get; set; }

        public MainWindowViewModel()
        {
            _testModelBuilder = new TestModelBuilder();
            TestModel.Create(_testModelBuilder);
            _testDiagram = new Diagramming.TestDiagram(_testModelBuilder);

            DiagramViewModel = new DiagramViewModel(_testDiagram, minZoom: 0.2, maxZoom: 5, initialZoom: 1);
            //DiagramViewModel.ShowSourceRequested += shape => Debug.WriteLine($"ShowSourceRequest: {shape.ModelItem.ToString()}");
            //DiagramViewModel.ShowModelItemsRequested += i => _testDiagram.ShowModelItems(i);

            AddCommand = new DelegateCommand(AddShapes);
            RemoveCommand = new DelegateCommand(RemoveShapes);
            ZoomToContentCommand = new DelegateCommand(ZoomToContent);
            CopyToClipboardCommand = new DelegateCommand(CopyToClipboardAsync);

            SelectedDpi = 300;
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

        private void AddShapes()
        {
            //var model = _testModelBuilder.AddClass($"Class{_testModelBuilder.CurrentModel.RootNodes.Count() + 1}");
            //_testDiagram.AddModelNode(model.RootNodes.Last());

            if (_modelItemGroupIndex == _testModelBuilder.ItemGroups.Count)
                return;

            var modelItemIds = _testModelBuilder.ItemGroups[_modelItemGroupIndex];
            var modelItems = modelItemIds.Select(i => _testModelBuilder.CurrentModel.RootNodes.FirstOrDefault(j => j.Id == i));

            _testDiagram.ShowModelItems(modelItems);
            _modelItemGroupIndex++;

            //_testDiagram.Save(@"c:\big.xml");

            ZoomToContent();
        }

        private void RemoveShapes()
        {
            //if (_nextToRemoveModelItemGroupIndex == _testDiagram.ModelItemGroups.Count)
            //    return;

            //_testDiagram.HideModelItems(_testDiagram.ModelItemGroups[_nextToRemoveModelItemGroupIndex]);
            //_nextToRemoveModelItemGroupIndex++;

            ZoomToContent();
        }

        private void ZoomToContent()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += (s, o) =>
            {
                timer.Stop();
                DiagramViewModel.ZoomToContent();
            };
            timer.Start();
        }

        private async void CopyToClipboardAsync()
        {
            using (var progressDialog = new ProgressDialog(Window, "TestHostApp", "Generating image.."))
            {
                progressDialog.ShowWithDelayAsync();

                var bitmapSource = await CreateDiagramImageAsync(progressDialog);
                SetImageToClipboard(bitmapSource);
            }
        }

        private async Task<BitmapSource> CreateDiagramImageAsync(ProgressDialog progressDialog)
        {
            try
            {
                var diagramImageCreator = new DataCloningDiagramImageCreator(DiagramViewModel, DiagramStlyeProvider);
                var cancellationToken = progressDialog.CancellationToken;

                return await Task.Factory.StartSTA(() =>
                    diagramImageCreator.CreateImage(SelectedDpi, 10, cancellationToken, progressDialog.Progress), cancellationToken);
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

        private static void SetImageToClipboard(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
                Clipboard.SetImage(bitmapSource);
        }

        private static void HandleOutOfMemory()
        {
            MessageBox.Show("Cannot export the image because it is too large. Please select a smaller DPI value.", "TestHostApp");
        }
    }
}
