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

        public IDiagramNode DiagramNode { get; }

        public List<RelatedEntityCueViewModel> RelatedEntityCueViewModels { get; }

        public DiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode)
              : base(diagram, diagramNode)
        {
            DiagramNode = diagramNode;

            _center = PointExtensions.Undefined;
            _topLeft = PointExtensions.Undefined;
            _size = Size.Empty;
            _name = diagramNode.Name;
            _fullName = diagramNode.FullName;

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
            return new DiagramNodeViewModel(Diagram, DiagramNode)
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

        private void OnRenamed(IDiagramNode diagramNode, string name, string fullName)
        {
            Name = name;
            FullName = fullName;
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
