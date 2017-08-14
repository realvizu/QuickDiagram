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
        private string _fullName;
        private string _description;
        private bool _descriptionExists;
        private bool _isDescriptionVisible;
        private string _nodeType;

        public IDiagramNode DiagramNode { get; }
        public List<RelatedNodeCueViewModel> RelatedNodeCueViewModels { get; }

        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;

        protected DiagramNodeViewModelBase(IArrangedDiagram diagram, IDiagramNode diagramNode, bool isDescriptionVisible,
            Size size, Point center, Point topLeft)
              : base(diagram, diagramNode)
        {
            DiagramNode = diagramNode;

            _size = size;
            _center = center;
            _topLeft = topLeft;
            _name = diagramNode.DisplayName;
            _fullName = diagramNode.FullName;
            _description = diagramNode.Description;
            _descriptionExists = !string.IsNullOrWhiteSpace(_description);
            _isDescriptionVisible = isDescriptionVisible;
            _nodeType = diagramNode.Type;

            RelatedNodeCueViewModels = CreateRelatedNodeCueViewModels();

            DiagramNode.CenterChanged += OnCenterChanged;
            DiagramNode.Renamed += OnRenamed;
        }

        public void Dispose()
        {
            DiagramNode.CenterChanged -= OnCenterChanged;
            DiagramNode.Renamed -= OnRenamed;

            foreach (var relatedNodeCueViewModel in RelatedNodeCueViewModels)
                relatedNodeCueViewModel.Dispose();
        }

        public bool IsStereotypeVisible => _nodeType != null;
        public string StereotypeText => IsStereotypeVisible ? $"<<{_nodeType.ToLower()}>>" : string.Empty;

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

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DescriptionExists
        {
            get { return _descriptionExists; }
            set
            {
                if (_descriptionExists != value)
                {
                    _descriptionExists = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsDescriptionVisible
        {
            get { return _isDescriptionVisible; }
            set
            {
                if (_isDescriptionVisible != value)
                {
                    _isDescriptionVisible = value;
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

        private void OnCenterChanged(IDiagramNode diagramNode, Point2D oldCenter, Point2D newCenter)
        {
            Center = newCenter.ToWpf();
        }

        private void OnSizeChanged(Size oldSize, Size newSize)
        {
            DiagramNode.Size = newSize.FromWpf();
        }

        private void OnRenamed(IDiagramNode diagramNode, string name, string fullName, string description)
        {
            Name = name;
            FullName = fullName;
            Description = description;
            DescriptionExists = !string.IsNullOrWhiteSpace(description);
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
