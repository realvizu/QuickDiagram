using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Graphs;
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
        private DiagramMutatorLayoutActionVisitor _testDiagramMutator;
        private DiagramBackwardsMutatorLayoutActionVisitor _testDiagramBackwardsMutator;
        private GraphBuilderLayoutActionVisitor _graphBuilderLayoutActionVisitor;
        private SequenceRecorderLayoutActionVisitor _sequenceRecorderLayoutActionVisitor;

        private int _totalNodeMoveCount;
        private int _frame;
        private string _frameLabel;
        private int _depth;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private List<ILayoutAction> AnimatedLayoutActions => _sequenceRecorderLayoutActionVisitor.LayoutActions;
        private LayoutActionGraph LayoutActionGraphForLastStep => _graphBuilderLayoutActionVisitor.LayoutActionGraph;

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
            _testDiagram.LayoutActionExecuted += RecordLayoutAction;

            _testDiagramMutator = new DiagramMutatorLayoutActionVisitor(_testDiagram);
            _testDiagramBackwardsMutator = new DiagramBackwardsMutatorLayoutActionVisitor(_testDiagram);
            _graphBuilderLayoutActionVisitor = new GraphBuilderLayoutActionVisitor();
            _sequenceRecorderLayoutActionVisitor = new SequenceRecorderLayoutActionVisitor();

            DiagramViewerControl.DataContext = _testDiagram;
            _modelEntities = _testDiagram.ModelItems.OfType<IModelEntity>().ToArray();

            FitToView();

            //_testDiagram.ModelItems.TakeUntil(i => i is TestModelEntity && ((TestModelEntity)i).Name == "IntermediateInterface")
            //    .ForEach(i => Add_OnClick(null, null));
        }

        private void RecordLayoutAction(object sender, ILayoutAction layoutAction)
        {
            layoutAction.AcceptVisitor(_graphBuilderLayoutActionVisitor);
            layoutAction.AcceptVisitor(_sequenceRecorderLayoutActionVisitor);
            TotalNodeMoveCount = _sequenceRecorderLayoutActionVisitor.TotalNodeMoveCount;
        }

        private void FitToView()
        {
            Dispatcher.BeginInvoke(new Action(() => DiagramViewerControl.FitDiagramToView()));
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            AnimatedLayoutActions.ForEach(PlayFrameForward);

            _sequenceRecorderLayoutActionVisitor.Clear();
            _graphBuilderLayoutActionVisitor.Clear();

            var modelItem = _testDiagram.ModelItems[_modelItemIndex];

            var modelEntity = modelItem as IModelEntity;
            if (modelEntity != null)
                _testDiagram.ShowNode(modelEntity);

            var modelRelationship = modelItem as IModelRelationship;
            if (modelRelationship != null)
                _testDiagram.ShowConnector((IModelRelationship)modelItem);

            DumpMoves(LayoutActionGraphForLastStep);

            if (_modelItemIndex < _testDiagram.ModelItems.Count - 1)
                _modelItemIndex++;

            FitToView();
            _frame = AnimatedLayoutActions.Count - 1;
            FrameLabel = _frame + " (To)";
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameForward(AnimatedLayoutActions[_frame]);

            if (_frame < AnimatedLayoutActions.Count - 1)
                _frame++;
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            PlayFrameBackward(AnimatedLayoutActions[_frame]);

            if (_frame > 0)
                _frame--;
        }

        private void PlayFrameForward(ILayoutAction layoutAction)
        {
            layoutAction.AcceptVisitor(_testDiagramMutator);

            FrameLabel = _frame + " (To)";
        }

        private void PlayFrameBackward(ILayoutAction layoutAction)
        {
            layoutAction.AcceptVisitor(_testDiagramBackwardsMutator);

            FrameLabel = _frame + " (From)";
        }

        private void DumpMoves(LayoutActionGraph layoutActionGraph)
        {
            Debug.WriteLine("-----------------------");
            _depth = 0;
            var searchAlgorithm = new DepthFirstSearchAlgorithm<ILayoutAction, IEdge<ILayoutAction>>(layoutActionGraph);
            searchAlgorithm.DiscoverVertex += OnDiscoverVertex;
            searchAlgorithm.FinishVertex += OnFinishVertex;
            searchAlgorithm.TreeEdge += OnTreeEdge;
            searchAlgorithm.ForwardOrCrossEdge += OnForwardOrCrossEdge;
            searchAlgorithm.Compute();
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

            DumpMoves(LayoutActionGraphForLastStep);

            _nextToRemoveModelItemIndex++;
        }

        private void LastLayoutActionGraph_OnClick(object sender, RoutedEventArgs e)
        {
            var textWindow = new TextWindow();
            textWindow.DataContext = textWindow;
            textWindow.Text = LayoutActionGraphForLastStep.Serialize();
            textWindow.Show();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

