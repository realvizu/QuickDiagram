using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;
using MoreLinq;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TestModel _testModel;
        private TestDiagram _testDiagram;
        private int _modelItemIndex;
        private int _totalNodeMoveCount;
        private int _frame;
        private string _frameLabel;
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public int TotalNodeMoveCount
        {
            get { return _totalNodeMoveCount; }
            set
            {
                if (_totalNodeMoveCount != value)
                {
                    _totalNodeMoveCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FrameLabel
        {
            get { return _frameLabel; }
            set
            {
                if (_frameLabel != value)
                {
                    _frameLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _testModel = TestModel.Create();
            _testDiagram = new TestDiagram(_testModel);
            DiagramViewerControl.DataContext = _testDiagram;

            FitToView();

            _testDiagram.ModelItems.TakeUntil(i => i is TestModelEntity && ((TestModelEntity)i).Name == "13")
                .ForEach(i => Next_OnClick(null, null));
        }

        private void FitToView()
        {
            Dispatcher.BeginInvoke(new Action(() => DiagramViewerControl.FitDiagramToView()));
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            _testDiagram.LastConnectorTriggeredNodeMoves.ForEach(PlayFrameForward);

            var modelItem = _testDiagram.ModelItems[_modelItemIndex];

            if (modelItem is IModelEntity)
                _testDiagram.ShowNode((IModelEntity)modelItem);
            else if (modelItem is IModelRelationship)
                _testDiagram.ShowConnector((IModelRelationship)modelItem);

            if (_modelItemIndex < _testDiagram.ModelItems.Count - 1)
                _modelItemIndex++;

            FitToView();
            TotalNodeMoveCount = _testDiagram.TotalNodeMoveCount;
            _frame = _testDiagram.LastConnectorTriggeredNodeMoves.Count - 1;
            FrameLabel = _frame + " (To)";
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameForward(_testDiagram.LastConnectorTriggeredNodeMoves[_frame]);

            if (_frame < _testDiagram.LastConnectorTriggeredNodeMoves.Count - 1)
                _frame++;
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameBackward(_testDiagram.LastConnectorTriggeredNodeMoves[_frame]);

            if (_frame > 0)
                _frame--;
        }

        private void PlayFrameForward(RectMoveEventArgs nodeMove)
        {
            ((DiagramNode)nodeMove.Rect).Center = nodeMove.ToCenter;
            FrameLabel = _frame + " (To)";
        }

        private void PlayFrameBackward(RectMoveEventArgs nodeMove)
        {
            ((DiagramNode)nodeMove.Rect).Center = nodeMove.FromCenter;
            FrameLabel = _frame + " (From)";
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

