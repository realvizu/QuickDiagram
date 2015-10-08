using System;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
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
        private int _shownModelEntityIndex = 0;
        private int _frame;

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

            FitToView();
        }

        private void FitToView()
        {
            Dispatcher.BeginInvoke(new Action(() => DiagramViewerControl.FitDiagramToView()));
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            _testDiagram.NodeMoves.ForEach(PlayFrameForward);

            var modelItem = _testDiagram.ModelItemsAddByClicks[_shownModelEntityIndex];

            if (modelItem is IModelEntity)
                _testDiagram.ShowNode((IModelEntity)modelItem);
            else if (modelItem is IModelRelationship)
                _testDiagram.ShowConnector((IModelRelationship)modelItem);

            if (_shownModelEntityIndex < _testDiagram.ModelItemsAddByClicks.Count - 1)
                _shownModelEntityIndex++;

            FitToView();
            _frame = _testDiagram.NodeMoves.Count - 1;
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameBackward(_testDiagram.NodeMoves[_frame]);

            if (_frame > 0)
            {
                _frame--;
                Frame.Text = _frame.ToString();
            }
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameForward(_testDiagram.NodeMoves[_frame]);

            if (_frame < _testDiagram.NodeMoves.Count - 1)
            {
                _frame++;
                Frame.Text = _frame.ToString();
            }
        }

        private void PlayFrameBackward(RectMove nodeMove)
        {
            ((DiagramNode)nodeMove.Node).Center = nodeMove.FromCenter;
        }

        private void PlayFrameForward(RectMove nodeMove)
        {
            ((DiagramNode)nodeMove.Node).Center = nodeMove.ToCenter;
        }
    }
}

