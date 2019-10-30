using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf.ViewModels;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for diagram nodes.
    /// </summary>
    public class DiagramNodeViewModel : DiagramShapeViewModelBase, IDiagramNodeUi
    {
        private string _name;
        private Point _center;
        private Point _topLeft;
        private Size _size;
        private Size _payloadAreaSize;
        private Size _childrenAreaSize;
        private bool _hasChildren;
        private Rect _animatedRect;

        [NotNull] protected IRelatedNodeTypeProvider RelatedNodeTypeProvider { get; }
        [NotNull] public IWpfFocusTracker<IDiagramShapeUi> FocusTracker { get; }
        [NotNull] [ItemNotNull] public List<RelatedNodeCueViewModel> RelatedNodeCueViewModels { get; }

        public event Action<IDiagramNode, Size2D> SizeChanged;
        public event Action<IDiagramNode, Size2D> PayloadAreaSizeChanged;
        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;
        public event Action<IDiagramNode> RemoveRequested;

        public DiagramNodeViewModel(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [NotNull] IFocusTracker<IDiagramShapeUi> focusTracker,
            [NotNull] IDiagramNode diagramNode)
            : base(modelService, diagramService, diagramNode)
        {
            PopulateFromDiagramNode(diagramNode);
            // Must NOT populate size from model because its value flows from the controls to the models.

            RelatedNodeTypeProvider = relatedNodeTypeProvider;
            FocusTracker = (IWpfFocusTracker<IDiagramShapeUi>)focusTracker;
            RelatedNodeCueViewModels = CreateRelatedNodeCueViewModels();

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override string Stereotype => DiagramNode.ModelNode.Stereotype.Name;

        public override void Dispose()
        {
            base.Dispose();

            DiagramService.DiagramChanged -= OnDiagramChanged;

            foreach (var relatedNodeCueViewModel in RelatedNodeCueViewModels)
                relatedNodeCueViewModel.Dispose();
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

        public Size ChildrenAreaSize
        {
            get { return _childrenAreaSize; }
            set
            {
                if (_childrenAreaSize != value)
                {
                    _childrenAreaSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasChildren
        {
            get { return _hasChildren; }
            set
            {
                if (_hasChildren != value)
                {
                    _hasChildren = value;
                    OnPropertyChanged();
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

        public void Remove() => RemoveRequested?.Invoke(DiagramNode);

        public void ShowRelatedModelNodes(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => ShowRelatedNodesRequested?.Invoke(ownerButton, modelNodes);

        public void ShowRelatedModelNodeSelector(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => RelatedNodeSelectorRequested?.Invoke(ownerButton, modelNodes);

        public override object CloneForImageExport()
        {
            var clone = new DiagramNodeViewModel(
                ModelService,
                DiagramService,
                RelatedNodeTypeProvider,
                FocusTracker,
                DiagramNode);

            SetPropertiesForImageExport(clone);
            
            return clone;
        }

        protected void SetPropertiesForImageExport([NotNull] DiagramNodeViewModel clone)
        {
            // For image export we must set those properties that are calculated on the normal UI.
            clone._size = _size;
            clone._payloadAreaSize = _payloadAreaSize;
            clone._center = _center;
            clone._topLeft = _topLeft;
        }

        public override IEnumerable<IMiniButton> CreateMiniButtons()
        {
            yield return new CloseMiniButtonViewModel(ModelService, DiagramService);

            foreach (var entityRelationType in GetRelatedNodeTypes())
                yield return new RelatedNodeMiniButtonViewModel(ModelService, DiagramService, entityRelationType);
        }

        [NotNull]
        private IEnumerable<RelatedNodeType> GetRelatedNodeTypes() => RelatedNodeTypeProvider.GetRelatedNodeTypes(ModelNode.Stereotype);

        private void OnDiagramChanged(DiagramEvent @event)
        {
            foreach (var change in @event.ShapeEvents)
                ProcessDiagramChange(change);
        }

        private void ProcessDiagramChange(DiagramShapeEventBase diagramShapeEvent)
        {
            if (diagramShapeEvent is DiagramNodeChangedEvent diagramNodeChangedEvent &&
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
            ChildrenAreaSize = diagramNode.ChildrenAreaSize.ToWpf();
            HasChildren = GetHasChildren(diagramNode);
            // Must NOT populate size from model because its value flows from the controls to the models.
        }

        private static bool GetHasChildren([NotNull] IDiagramNode diagramNode)
        {
            return diagramNode.ChildrenAreaSize.IsDefined && diagramNode.ChildrenAreaSize != Size2D.Zero;
        }

        [NotNull]
        [ItemNotNull]
        private List<RelatedNodeCueViewModel> CreateRelatedNodeCueViewModels()
        {
            return GetRelatedNodeTypes()
                .Select(i => new RelatedNodeCueViewModel(ModelService, DiagramService, DiagramNode, i))
                .ToList();
        }
    }
}