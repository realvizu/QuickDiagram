using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;
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
        private IModelEntity[] _modelEntities;
        private int _modelItemIndex;
        private int _nextToRemoveModelItemIndex;

        private TestDiagram _testDiagram;
        private int _totalNodeMoveCount;

        private List<LayoutActionGraph> _layoutActionTreesForLastStep;
        private List<ILayoutAction> _animatedLayoutActions;
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

            _layoutActionTreesForLastStep = new List<LayoutActionGraph>();
            _animatedLayoutActions = new List<ILayoutAction>();

            _testModel = TestModel.Create();
            _testDiagram = new TestDiagram(_testModel);
            _testDiagram.LayoutActionExecuted += RecordLayoutAction;

            DiagramViewerControl.DataContext = _testDiagram;
            _modelEntities = _testDiagram.ModelItems.OfType<IModelEntity>().ToArray();

            FitToView();

            //_testDiagram.ModelItems.TakeUntil(i => i is TestModelEntity && ((TestModelEntity)i).Name == "IntermediateInterface")
            //    .ForEach(i => Add_OnClick(null, null));
        }

        private void RecordLayoutAction(object sender, ILayoutAction layoutAction)
        {
            if (layoutAction.CausingLayoutAction == null)
                _layoutActionTreesForLastStep.Add(new LayoutActionGraph());

            _layoutActionTreesForLastStep.Last().AddVertex(layoutAction);

            if (layoutAction.CausingLayoutAction != null)
            {
                var edge = new Edge<ILayoutAction>(layoutAction.CausingLayoutAction, layoutAction);
                _layoutActionTreesForLastStep.Last().AddEdge(edge);
            }

            if (layoutAction is IMoveDiagramNodeAction ||
                layoutAction is IRerouteDiagramConnectorAction)
            {
                _animatedLayoutActions.Add(layoutAction);

                if (layoutAction is IMoveDiagramNodeAction)
                    TotalNodeMoveCount++;
            }
        }

        private void FitToView()
        {
            Dispatcher.BeginInvoke(new Action(() => DiagramViewerControl.FitDiagramToView()));
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            _animatedLayoutActions.ForEach(PlayFrameForward);
            _animatedLayoutActions.Clear();
            _layoutActionTreesForLastStep.Clear();

            var modelItem = _testDiagram.ModelItems[_modelItemIndex];

            var modelEntity = modelItem as IModelEntity;
            if (modelEntity != null)
                _testDiagram.ShowNode(modelEntity);

            var modelRelationship = modelItem as IModelRelationship;
            if (modelRelationship != null)
                _testDiagram.ShowConnector((IModelRelationship)modelItem);

            DumpMoves(_layoutActionTreesForLastStep);

            if (_modelItemIndex < _testDiagram.ModelItems.Count - 1)
                _modelItemIndex++;

            FitToView();
            _frame = _animatedLayoutActions.Count - 1;
            FrameLabel = _frame + " (To)";
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameForward(_animatedLayoutActions[_frame]);

            if (_frame < _animatedLayoutActions.Count - 1)
                _frame++;
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameBackward(_animatedLayoutActions[_frame]);

            if (_frame > 0)
                _frame--;
        }

        private void PlayFrameForward(ILayoutAction layoutAction)
        {
            ApplyFrame(layoutAction, i => i.To, i => i.NewRoute);

            FrameLabel = _frame + " (To)";
        }

        private void PlayFrameBackward(ILayoutAction layoutAction)
        {
            ApplyFrame(layoutAction, i => i.From, i => i.OldRoute);

            FrameLabel = _frame + " (From)";
        }

        private static void ApplyFrame(ILayoutAction layoutAction,
            Func<IMoveDiagramNodeAction, Point2D> vertexCenterAccessor,
            Func<IRerouteDiagramConnectorAction, Route> connectorRouteAccessor)
        {
            var moveDiagramNodeAction = layoutAction as IMoveDiagramNodeAction;
            if (moveDiagramNodeAction != null)
                moveDiagramNodeAction.DiagramNode.Center = vertexCenterAccessor(moveDiagramNodeAction);

            var rerouteDiagramConnectorAction = layoutAction as IRerouteDiagramConnectorAction;
            if (rerouteDiagramConnectorAction != null)
                rerouteDiagramConnectorAction.DiagramConnector.RoutePoints = connectorRouteAccessor(rerouteDiagramConnectorAction);
        }

        private void DumpMoves(List<LayoutActionGraph> layoutActionTrees)
        {
            Debug.WriteLine("-----------------------");
            foreach (var layoutActionTree in layoutActionTrees)
            {
                _depth = 0;
                var searchAlgorithm = new DepthFirstSearchAlgorithm<ILayoutAction, IEdge<ILayoutAction>>(layoutActionTree);
                searchAlgorithm.DiscoverVertex += OnDiscoverVertex;
                searchAlgorithm.FinishVertex += OnFinishVertex;
                searchAlgorithm.TreeEdge += OnTreeEdge;
                searchAlgorithm.ForwardOrCrossEdge += OnForwardOrCrossEdge;
                searchAlgorithm.Compute();
            }
        }

        private void OnForwardOrCrossEdge(IEdge<ILayoutAction> e)
        {
            Debug.WriteLine($"{GetIndent()}--> {e.Target}");
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

            DumpMoves(_layoutActionTreesForLastStep);

            _nextToRemoveModelItemIndex++;
        }

        private void LastLayoutActionGraph_OnClick(object sender, RoutedEventArgs e)
        {
            var textWindow = new TextWindow();
            textWindow.DataContext = textWindow;
            textWindow.Text = _layoutActionTreesForLastStep.Last().Serialize();
            textWindow.Show();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

