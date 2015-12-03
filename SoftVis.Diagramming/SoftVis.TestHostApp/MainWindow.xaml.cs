using System;
using System.Collections.Generic;
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
        private int _modelItemGroupIndex;
        private int _nextToRemoveModelItemGroupIndex;

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
            if (_modelItemGroupIndex == _testDiagram.ModelItemGroups.Count)
                return;

            foreach (var modelItem in _testDiagram.ModelItemGroups[_modelItemGroupIndex])
                AddModelItem(modelItem);

            _modelItemGroupIndex++;

            FitToView();
        }

        private void AddModelItem(IModelItem modelItem)
        {
            var modelEntity = modelItem as IModelEntity;
            if (modelEntity != null)
                _testDiagram.ShowNode(modelEntity);

            var modelRelationship = modelItem as IModelRelationship;
            if (modelRelationship != null)
                _testDiagram.ShowConnector((IModelRelationship)modelItem);
        }

        private void Remove_OnClick(object sender, RoutedEventArgs e)
        {
            if (_nextToRemoveModelItemGroupIndex == _testDiagram.ModelItemGroups.Count)
                return;

            var modelEntitiesToRemove = _testDiagram.ModelItemGroups[_nextToRemoveModelItemGroupIndex].OfType<IModelEntity>();
            foreach (var modelEntity in modelEntitiesToRemove)
                _testDiagram.HideNode(modelEntity);

            _nextToRemoveModelItemGroupIndex++;
        }
    }
}

