using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for view models that define the visible properties of a diagram node.
    /// </summary>
    public abstract class DiagramNodeViewModelBase : DiagramShapeViewModelBase, ICloneable, IDisposable
    {
        private readonly object _positionUpdateLock = new object();

        private Point _center;
        private Point _topLeft;
        private Size _size;
        private Rect _animatedRect;
        private string _name;

        public IDiagramNode DiagramNode { get; }
        public List<RelatedNodeCueViewModel> RelatedNodeCueViewModels { get; }

        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;

        protected DiagramNodeViewModelBase(IArrangedDiagram diagram, IDiagramNode diagramNode, Size size, Point center, Point topLeft)
              : base(diagram, diagramNode)
        {
            DiagramNode = diagramNode;

            _size = size;
            _center = center;
            _topLeft = topLeft;
            _name = diagramNode.Name;

            RelatedNodeCueViewModels = CreateRelatedNodeCueViewModels();

            DiagramNode.CenterChanged += OnCenterChanged;
            DiagramNode.ModelNodeUpdated += OnModelNodeUpdated;
        }

        public void Dispose()
        {
            DiagramNode.CenterChanged -= OnCenterChanged;
            DiagramNode.ModelNodeUpdated -= OnModelNodeUpdated;

            foreach (var relatedNodeCueViewModel in RelatedNodeCueViewModels)
                relatedNodeCueViewModel.Dispose();
        }

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
                lock (_positionUpdateLock)
                {
                    if (_center != value)
                    {
                        _center = value;
                        OnPropertyChanged();

                        TopLeft = CenterToTopLeft(_center, _size);
                    }
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
                lock (_positionUpdateLock)
                {
                    if (_size != value)
                    {
                        var oldSize = _size;
                        _size = value;
                        OnPropertyChanged();

                        OnSizeChanged(oldSize, value);
                        TopLeft = CenterToTopLeft(_center, _size);
                    }
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

        public void ShowRelatedNodes(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes) => 
            ShowRelatedNodesRequested?.Invoke(ownerButton, modelNodes);

        public void ShowRelatedNodeSelector(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes) =>
            RelatedNodeSelectorRequested?.Invoke(ownerButton, modelNodes);

        public override IEnumerable<MiniButtonViewModelBase> CreateMiniButtonViewModels()
        {
            yield return new CloseMiniButtonViewModel(Diagram);

            foreach (var entityRelationType in GetRelatedNodeTypes())
                yield return new RelatedNodeMiniButtonViewModel(Diagram, entityRelationType);
        }

        protected virtual IEnumerable<RelatedNodeType> GetRelatedNodeTypes()
        {
            yield break;
        }

        protected virtual void OnModelNodeUpdated(IDiagramNode diagramNode, IModelNode modelNode)
        {
            Name = modelNode.Name;
        }

        private void OnCenterChanged(IDiagramNode diagramNode, Point2D oldCenter, Point2D newCenter)
        {
            Center = newCenter.ToWpf();
        }

        private void OnSizeChanged(Size oldSize, Size newSize)
        {
            DiagramNode.Size = newSize.FromWpf();
        }

        private List<RelatedNodeCueViewModel> CreateRelatedNodeCueViewModels()
        {
            return GetRelatedNodeTypes()
                .Select(i => new RelatedNodeCueViewModel(Diagram, DiagramNode, i))
                .ToList();
        }

        private Point CenterToTopLeft(Point center, Size size)
        {
            return new Point(_center.X - size.Width / 2, center.Y - size.Height / 2);
        }
    }
}
