using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of a diagram node.
    /// </summary>
    public sealed class DiagramNodeViewModel : DiagramShapeViewModelBase, ICloneable, IDisposable
    {
        private Point _topLeft;
        private Size _size;
        private string _name;
        private string _fullName;

        public IDiagramNode DiagramNode { get; }

        public List<RelatedEntityCueViewModel> RelatedEntityCueViewModels { get; }

        public DelegateCommand DoubleClickCommand { get; }

        public DiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode)
              : base(diagram, diagramNode)
        {
            DiagramNode = diagramNode;

            _topLeft = PointExtensions.Undefined;
            _size = Size.Empty;
            _name = diagramNode.Name;
            _fullName = diagramNode.FullName;

            RelatedEntityCueViewModels = CreateRelatedEntityCueViewModels();

            DoubleClickCommand = new DelegateCommand(RequestShowSource);

            DiagramNode.TopLeftChanged += OnTopLeftChanged;
            DiagramNode.Renamed += OnRenamed;
        }

        public void Dispose()
        {
            DiagramNode.TopLeftChanged -= OnTopLeftChanged;
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

        public Point TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (_topLeft != value)
                {
                    _topLeft = value;
                    OnPropertyChanged();
                    OnPropertyChanged("X");
                    OnPropertyChanged("Y");
                }
            }
        }

        public double X => TopLeft.X;
        public double Y => TopLeft.Y;

        public Size Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();
                    OnSizeChanged(value);
                    OnPropertyChanged("Width");
                    OnPropertyChanged("Height");
                }
            }
        }

        public double Width => Size.Width;
        public double Height => Size.Height;

        public object Clone()
        {
            return new DiagramNodeViewModel(Diagram, DiagramNode)
            {
                _size = _size,
                _topLeft = _topLeft,
            };
        }

        private void OnTopLeftChanged(IDiagramNode diagramNode, Point2D oldTopLeft, Point2D newTopLeft)
        {
            TopLeft = newTopLeft.ToWpf();
        }

        private void OnSizeChanged(Size newSize)
        {
            DiagramNode.Size = newSize.FromWpf();
        }

        private void OnRenamed(IDiagramNode diagramNode, string name, string fullName)
        {
            Name = name;
            FullName = fullName;
        }

        private void RequestShowSource()
        {
            Diagram.ShowSource(DiagramNode);
        }

        private List<RelatedEntityCueViewModel> CreateRelatedEntityCueViewModels()
        {
            return Diagram.GetEntityRelationTypes()
                .Select(i => new RelatedEntityCueViewModel(Diagram, DiagramNode, i))
                .ToList();
        }
    }
}
