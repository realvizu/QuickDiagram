using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;
using MoreLinq;
using QuickGraph;
using QuickGraph.Algorithms.Search;

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
        private IModelEntity[] _modelEntities;
        private int _nextToRemoveModelItemIndex;
        private int _totalNodeMoveCount;
        private int _frame;
        private string _frameLabel;
        private int _depth;

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
            _testDiagram.LastLayoutActionVertexMoves.ForEach(PlayFrameForward);

            var modelItem = _testDiagram.ModelItems[_modelItemIndex];

            var modelEntity = modelItem as IModelEntity;
            if (modelEntity != null)
                _testDiagram.ShowNode(modelEntity);

            var modelRelationship = modelItem as IModelRelationship;
            if (modelRelationship != null)
                _testDiagram.ShowConnector((IModelRelationship)modelItem);

            DumpMoves(_testDiagram.LastLayoutActionGraph);

            if (_modelItemIndex < _testDiagram.ModelItems.Count - 1)
                _modelItemIndex++;

            FitToView();
            TotalNodeMoveCount = _testDiagram.TotalVertexMoveCount;
            _frame = _testDiagram.LastLayoutActionVertexMoves.Length - 1;
            FrameLabel = _frame + " (To)";
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameForward(_testDiagram.LastLayoutActionVertexMoves[_frame]);

            if (_frame < _testDiagram.LastLayoutActionVertexMoves.Length - 1)
                _frame++;
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameBackward(_testDiagram.LastLayoutActionVertexMoves[_frame]);

            if (_frame > 0)
                _frame--;
        }

        private void PlayFrameForward(IVertexMoveAction vertexMove)
        {
            if (vertexMove.DiagramNode != null)
                vertexMove.DiagramNode.Center = vertexMove.To;

            FrameLabel = _frame + " (To)";
        }

        private void PlayFrameBackward(IVertexMoveAction vertexMove)
        {
            if (vertexMove.DiagramNode != null)
                vertexMove.DiagramNode.Center = vertexMove.From;

            FrameLabel = _frame + " (From)";
        }

        private void DumpMoves(ILayoutActionGraph layoutActionGraph)
        {
            Debug.WriteLine("-----------------------");

            _depth = 0;
            var searchAlgorithm = new DepthFirstSearchAlgorithm<ILayoutAction, ILayoutActionEdge>(layoutActionGraph);
            searchAlgorithm.DiscoverVertex += OnDiscoverVertex;
            searchAlgorithm.FinishVertex += OnFinishVertex;
            searchAlgorithm.TreeEdge += OnTreeEdge;
            searchAlgorithm.ForwardOrCrossEdge += OnForwardOrCrossEdge;
            searchAlgorithm.Compute();
        }

        private void OnForwardOrCrossEdge(IEdge<ILayoutAction> e)
        {
            Debug.WriteLine($"{GetIndent()}ForwardOrCrossEdge to {e.Target}");
        }

        private void OnTreeEdge(IEdge<ILayoutAction> edge)
        {
            _depth++;
        }

        private void OnDiscoverVertex(ILayoutAction layoutAction)
        {
            Debug.WriteLine($"{GetIndent()}{layoutAction}");
        }

        private void OnFinishVertex(ILayoutAction layoutAction)
        {
            _depth--;
        }

        private string GetIndent()
        {
            return new string(' ', _depth * 2);
        }

        private void Remove_OnClick(object sender, RoutedEventArgs e)
        {
            _testDiagram.HideNode(_modelEntities[_nextToRemoveModelItemIndex]);

            DumpMoves(_testDiagram.LastLayoutActionGraph);

            _nextToRemoveModelItemIndex++;
        }

        private void LastLayoutActionGraph_OnClick(object sender, RoutedEventArgs e)
        {
            var textWindow = new TextWindow();
            textWindow.DataContext = textWindow;
            textWindow.Text = _testDiagram.LastLayoutActionGraph.Serialize();
            textWindow.Show();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

