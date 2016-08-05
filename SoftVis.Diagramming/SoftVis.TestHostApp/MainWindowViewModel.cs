using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.TestHostApp.TestData;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.TestHostApp
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly TestModel _testModel;
        private readonly TestDiagram _testDiagram;

        private int _modelItemGroupIndex;
        private int _nextToRemoveModelItemGroupIndex;
        private double _selectedDpi;

        public DiagramViewModel DiagramViewModel { get; }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ZoomToContentCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        public MainWindowViewModel()
        {
            _testModel = new TestModelBuilder().Create();
            //_testModel = new BigTestModelBuilder().Create(4, 4);

            _testDiagram = new TestDiagram(_testModel);
            _testDiagram.ShapeActivated += (sender, shape) => Debug.WriteLine($"Activated: {shape.ModelItem.ToString()}");

            DiagramViewModel = new DiagramViewModel(_testDiagram, minZoom: 0.2, maxZoom: 5, initialZoom: 1);
            
            AddCommand = new DelegateCommand(AddShapes);
            RemoveCommand = new DelegateCommand(RemoveShapes);
            ZoomToContentCommand = new DelegateCommand(ZoomToContent);
            CopyToClipboardCommand = new DelegateCommand(CopyToClipboard);

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
            if (_modelItemGroupIndex == _testDiagram.ModelItemGroups.Count)
                return;

            _testDiagram.ShowItems(_testDiagram.ModelItemGroups[_modelItemGroupIndex]);
            _modelItemGroupIndex++;

            //_testDiagram.Save(@"c:\big.xml");

            ZoomToContent();
        }

        private void RemoveShapes()
        {
            if (_nextToRemoveModelItemGroupIndex == _testDiagram.ModelItemGroups.Count)
                return;

            _testDiagram.HideItems(_testDiagram.ModelItemGroups[_nextToRemoveModelItemGroupIndex]);
            _nextToRemoveModelItemGroupIndex++;

            ZoomToContent();
        }

        private void ZoomToContent() => DiagramViewModel.ZoomToContent();
        private void CopyToClipboard() => DiagramViewModel.GetDiagramImage(SelectedDpi, Clipboard.SetImage);
    }
}
