using System;
using System.Windows;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestModel _testModel;
        private TestDiagram _testDiagram;
        private int shownModelEntityIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _testModel = TestModel.Create();
            _testDiagram = new TestDiagram(_testModel);
            DiagramViewerControl.DataContext = _testDiagram;

            Dispatcher.BeginInvoke(new Action(() => DiagramViewerControl.FitDiagramToView()));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var modelItem = _testDiagram.ModelItemsAddByClicks[shownModelEntityIndex];

            if (modelItem is IModelEntity)
                _testDiagram.ShowNode((IModelEntity)modelItem);
            else if (modelItem is IModelRelationship)
                _testDiagram.ShowConnector((IModelRelationship)modelItem);

            if (shownModelEntityIndex < _testDiagram.ModelItemsAddByClicks.Count - 1)
                shownModelEntityIndex++;
        }
    }
}
