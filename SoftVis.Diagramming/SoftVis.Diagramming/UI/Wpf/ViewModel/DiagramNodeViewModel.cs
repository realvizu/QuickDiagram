using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of a diagram node.
    /// </summary>
    public sealed class DiagramNodeViewModel : DiagramShapeViewModelBase, ICloneable, IDisposable
    {
        private readonly object _positionUpdateLock = new object();

        private Point _center;
        private Point _topLeft;
        private Size _size;
        private string _name;
        private string _fullName;
        private string _description;
        private bool _descriptionExists;
        private bool _isDescriptionVisible;

        public IDiagramNode DiagramNode { get; }

        public List<RelatedEntityCueViewModel> RelatedEntityCueViewModels { get; }

        public DiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode, bool isDescriptionVisible)
              : base(diagram, diagramNode)
        {
            DiagramNode = diagramNode;

            _center = PointExtensions.Undefined;
            _topLeft = PointExtensions.Undefined;
            _size = Size.Empty;
            _name = diagramNode.Name;
            _fullName = diagramNode.FullName;
            _description = diagramNode.Description;
            _descriptionExists = !string.IsNullOrWhiteSpace(_description);
            _isDescriptionVisible = isDescriptionVisible;

            RelatedEntityCueViewModels = CreateRelatedEntityCueViewModels();

            DiagramNode.CenterChanged += OnCenterChanged;
            DiagramNode.Renamed += OnRenamed;
        }

        public void Dispose()
        {
            DiagramNode.CenterChanged -= OnCenterChanged;
            DiagramNode.Renamed -= OnRenamed;

            foreach (var relatedEntityCueViewModel in RelatedEntityCueViewModels)
                relatedEntityCueViewModel.Dispose();
        }

        public IModelEntity ModelEntity => DiagramNode.ModelEntity;
        public ModelEntityStereotype Stereotype => ModelEntity.Stereotype;
        public bool IsStereotypeVisible => Stereotype != ModelEntityStereotype.None;
        public string StereotypeText => IsStereotypeVisible ? $"<<{Stereotype.Name.ToLower()}>>" : string.Empty;

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

        public object Clone()
        {
            return new DiagramNodeViewModel(Diagram, DiagramNode, _isDescriptionVisible)
            {
                _size = _size,
                _center = _center,
                _topLeft = _topLeft,
            };
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

        private List<RelatedEntityCueViewModel> CreateRelatedEntityCueViewModels()
        {
            return Diagram.GetEntityRelationTypes()
                .Select(i => new RelatedEntityCueViewModel(Diagram, DiagramNode, i))
                .ToList();
        }

        private Point CenterToTopLeft(Point center, Size size)
        {
            return new Point(_center.X - size.Width / 2, center.Y - size.Height / 2);
        }
    }
}
