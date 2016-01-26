using System.Windows.Input;
using Codartis.SoftVis.TestHostApp.TestData;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.TestHostApp
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly TestModel _testModel;
        private readonly TestDiagram _testDiagram;
        private DiagramViewModel _diagramViewModel;

        private int _modelItemGroupIndex;
        private int _nextToRemoveModelItemGroupIndex;

        private ICommand _addCommand;
        private ICommand _removeCommand;

        public MainWindowViewModel()
        {
            _testModel = TestModel.Create();
            //_testModel = TestModel.CreateBig(40, 2);

            var diagramStyleProvider = new TestConnectorTypeResolver();
            _testDiagram = new TestDiagram(diagramStyleProvider, _testModel);

            var diagramBehaviourProvider = new TestDiagramBehaviourProvider();
            _diagramViewModel = new DiagramViewModel(_testModel, _testDiagram, diagramBehaviourProvider,
                0.2, 5, 1);
            
            AddCommand = new DelegateCommand(AddShapes);
            RemoveCommand = new DelegateCommand(RemoveShapes);
        }

        public ICommand AddCommand
        {
            get { return _addCommand; }
            set
            {
                _addCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand RemoveCommand
        {
            get { return _removeCommand; }
            set
            {
                _removeCommand = value;
                OnPropertyChanged();
            }
        }

        public DiagramViewModel DiagramViewModel
        {
            get { return _diagramViewModel; }
            set
            {
                _diagramViewModel = value;
                OnPropertyChanged();
            }
        }

        private void ZoomToContent()
        {
            _diagramViewModel.ZoomToContent();
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
        }
    }
}
