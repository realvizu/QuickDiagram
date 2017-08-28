using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for view models that define the visible properties of a diagram node.
    /// </summary>
    public abstract class DiagramNodeViewModelBase : DiagramShapeViewModelBase, ICloneable
    {
        private string _name;
        private Point _center;
        private Point _topLeft;
        private Size _size;
        private Rect _animatedRect;

        public List<RelatedNodeCueViewModel> RelatedNodeCueViewModels { get; }

        public event Action<IDiagramNode, Size> SizeChanged;
        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;
        public event Action<IDiagramNode> RemoveRequested;

        protected DiagramNodeViewModelBase(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore,
            IDiagramNode diagramNode)
              : base(modelStore, diagramStore, diagramNode)
        {
            PopulateFromDiagramNode(diagramNode);

            RelatedNodeCueViewModels = CreateRelatedNodeCueViewModels();

            DiagramStore.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            DiagramStore.DiagramChanged -= OnDiagramChanged;

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
                    SizeChanged?.Invoke(DiagramNode, _size);
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

        public abstract object Clone();

        public void Remove() => RemoveRequested?.Invoke(DiagramNode);

        public void ShowRelatedModelNodes(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes) =>
            ShowRelatedNodesRequested?.Invoke(ownerButton, modelNodes);

        public void ShowRelatedModelNodeSelector(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes) =>
            RelatedNodeSelectorRequested?.Invoke(ownerButton, modelNodes);

        public override IEnumerable<MiniButtonViewModelBase> CreateMiniButtonViewModels()
        {
            yield return new CloseMiniButtonViewModel(ModelStore, DiagramStore);

            foreach (var entityRelationType in GetRelatedNodeTypes())
                yield return new RelatedNodeMiniButtonViewModel(ModelStore, DiagramStore, entityRelationType);
        }

        protected abstract IEnumerable<RelatedNodeType> GetRelatedNodeTypes();

        protected virtual void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            if (diagramEvent is DiagramNodeChangedEventBase diagramNodeChangedEvent
                && DiagramNodeIdEqualityComparer.Instance.Equals(diagramNodeChangedEvent.DiagramNode, DiagramNode))
            {
                PopulateFromDiagramNode(diagramNodeChangedEvent.DiagramNode);
            }
        }

        private void PopulateFromDiagramNode(IDiagramNode diagramNode)
        {
            // Must NOT populate size from model because its value flows from the controls to the models.
            Center = diagramNode.Center.ToWpf();
            TopLeft = diagramNode.TopLeft.ToWpf();
            Name = diagramNode.Name;
        }

        private List<RelatedNodeCueViewModel> CreateRelatedNodeCueViewModels()
        {
            return GetRelatedNodeTypes()
                .Select(i => new RelatedNodeCueViewModel(ModelStore, DiagramStore, DiagramNode, i))
                .ToList();
        }
    }
}
