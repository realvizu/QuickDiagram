using System;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private TestModel _testModel;
        private IModelEntity[] _modelEntities;
        private int _modelItemIndex;
        private int _nextToRemoveModelItemIndex;

        private TestDiagram _testDiagram;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _testModel = TestModel.Create();
            _testDiagram = new TestDiagram(_testModel);

            DiagramViewerControl.DataContext = _testDiagram;
            _modelEntities = _testDiagram.ModelItems.OfType<IModelEntity>().ToArray();

            FitToView();

            //_testDiagram.ModelItems.TakeUntil(i => i is TestModelEntity && ((TestModelEntity)i).Name == "IntermediateInterface")
            //    .ForEach(i => Add_OnClick(null, null));
        }

        private void FitToView()
        {
            Dispatcher.BeginInvoke(new Action(() => DiagramViewerControl.FitDiagramToView()));
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            var modelItem = _testDiagram.ModelItems[_modelItemIndex];

            var modelEntity = modelItem as IModelEntity;
            if (modelEntity != null)
                _testDiagram.ShowNode(modelEntity);

            var modelRelationship = modelItem as IModelRelationship;
            if (modelRelationship != null)
                _testDiagram.ShowConnector((IModelRelationship)modelItem);

            if (_modelItemIndex < _testDiagram.ModelItems.Count - 1)
                _modelItemIndex++;

            FitToView();
        }

        private void Remove_OnClick(object sender, RoutedEventArgs e)
        {
            _testDiagram.HideNode(_modelEntities[_nextToRemoveModelItemIndex]);

            _nextToRemoveModelItemIndex++;
        }
    }
}

