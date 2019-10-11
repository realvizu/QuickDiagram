using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf.Collections;
using Codartis.Util.UI.Wpf.ViewModels;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for diagram nodes.
    /// </summary>
    public sealed class DiagramNodeViewModel : DiagramShapeViewModelBase, IDiagramNodeUi
    {
        private string _name;
        private Point _center;
        private Point _topLeft;
        private Size _size;
        private Size _payloadAreaSize;
        private Rect _animatedRect;

        [NotNull] private readonly IRelatedNodeTypeProvider _relatedNodeTypeProvider;
        public IWpfFocusTracker<IDiagramShapeUi> FocusTracker { get; }
        public List<RelatedNodeCueViewModel> RelatedNodeCueViewModels { get; }
        public ThreadSafeObservableCollection<DiagramNodeViewModel> ChildNodes { get; }

        public event Action<IDiagramNode, Size2D> SizeChanged;
        public event Action<IDiagramNode, Size2D> PayloadAreaSizeChanged;
        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;
        public event Action<IDiagramNode> RemoveRequested;

        public DiagramNodeViewModel(
            IModelService modelService,
            IDiagramService diagramService,
            IRelatedNodeTypeProvider relatedNodeTypeProvider,
            IFocusTracker<IDiagramShapeUi> focusTracker,
            IDiagramNode diagramNode)
            : base(modelService, diagramService, diagramNode)
        {
            Name = diagramNode.ModelNode.Name;
            Center = diagramNode.Center.ToWpf();
            TopLeft = diagramNode.TopLeft.ToWpf();
            // Must NOT populate size from model because its value flows from the controls to the models.

            _relatedNodeTypeProvider = relatedNodeTypeProvider;
            FocusTracker = (IWpfFocusTracker<IDiagramShapeUi>)focusTracker;
            RelatedNodeCueViewModels = CreateRelatedNodeCueViewModels();
            ChildNodes = new ThreadSafeObservableCollection<DiagramNodeViewModel>();

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            DiagramService.DiagramChanged -= OnDiagramChanged;

            foreach (var relatedNodeCueViewModel in RelatedNodeCueViewModels)
                relatedNodeCueViewModel.Dispose();

            foreach (var childNode in ChildNodes)
                childNode.Dispose();
        }

        public IDiagramNode DiagramNode => (IDiagramNode)DiagramShape;
        public IModelNode ModelNode => DiagramNode.ModelNode;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public Point Center
        {
            get { return _center; }
            set
            {
                if (_center != value)
                {
                    _center = value;
                    OnPropertyChanged();
                }
            }
        }

        public Point TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (_topLeft != value)
                {
                    _topLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public Size Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();

                    // This property binds to its control as OneWayToSource and propagates size changes to parent viewmodels.
                    SizeChanged?.Invoke(DiagramNode, _size.FromWpf());
                }
            }
        }

        public Size PayloadAreaSize
        {
            get { return _payloadAreaSize; }
            set
            {
                if (_payloadAreaSize != value)
                {
                    _payloadAreaSize = value;
                    OnPropertyChanged();

                    // This property binds to its control as OneWayToSource and propagates size changes to parent viewmodels.
                    PayloadAreaSizeChanged?.Invoke(DiagramNode, _payloadAreaSize.FromWpf());
                }
            }
        }

        public Rect AnimatedRect
        {
            get { return _animatedRect; }
            set
            {
                if (_animatedRect != value)
                {
                    _animatedRect = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AddChildNode(IDiagramNodeUi childNode)
        {
            ChildNodes.Add(childNode as DiagramNodeViewModel);
        }

        public void Remove() => RemoveRequested?.Invoke(DiagramNode);

        public void ShowRelatedModelNodes(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => ShowRelatedNodesRequested?.Invoke(ownerButton, modelNodes);

        public void ShowRelatedModelNodeSelector(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => RelatedNodeSelectorRequested?.Invoke(ownerButton, modelNodes);

        public override object Clone()
        {
            return new DiagramNodeViewModel(ModelService, DiagramService, _relatedNodeTypeProvider, FocusTracker, DiagramNode);
        }

        public override IEnumerable<IMiniButton> CreateMiniButtons()
        {
            yield return new CloseMiniButtonViewModel(ModelService, DiagramService);

            foreach (var entityRelationType in GetRelatedNodeTypes())
                yield return new RelatedNodeMiniButtonViewModel(ModelService, DiagramService, entityRelationType);
        }

        private IEnumerable<RelatedNodeType> GetRelatedNodeTypes() => _relatedNodeTypeProvider.GetRelatedNodeTypes(ModelNode.Stereotype);

        private void OnDiagramChanged(DiagramEvent @event)
        {
            foreach (var change in @event.ShapeEvents)
                ProcessDiagramChange(change);
        }

        private void ProcessDiagramChange(DiagramShapeEventBase diagramShapeEvent)
        {
            if (diagramShapeEvent is DiagramNodeChangedEventBase diagramNodeChangedEvent &&
                DiagramNodeIdEqualityComparer.Instance.Equals(diagramNodeChangedEvent.NewNode, DiagramNode))
            {
                DiagramShape = diagramNodeChangedEvent.NewNode;
                PopulateFromDiagramNode(diagramNodeChangedEvent.NewNode);
            }
        }

        private void PopulateFromDiagramNode(IDiagramNode diagramNode)
        {
            Name = diagramNode.ModelNode.Name;
            Center = diagramNode.Center.ToWpf();
            TopLeft = diagramNode.TopLeft.ToWpf();
            // Must NOT populate size from model because its value flows from the controls to the models.
        }

        private List<RelatedNodeCueViewModel> CreateRelatedNodeCueViewModels()
        {
            return GetRelatedNodeTypes()
                .Select(i => new RelatedNodeCueViewModel(ModelService, DiagramService, DiagramNode, i))
                .ToList();
        }
    }
}