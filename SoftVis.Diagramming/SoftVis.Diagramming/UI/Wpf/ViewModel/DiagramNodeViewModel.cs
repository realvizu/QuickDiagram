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
    public sealed class DiagramNodeViewModel : DiagramShapeViewModelBase, ICloneable
    {
        private Point _topLeft;
        private Size _size;

        public IDiagramNode DiagramNode { get; }

        public DelegateCommand FocusCommand { get; }
        public DelegateCommand DoubleClickCommand { get; }

        public List<RelatedEntityCueViewModel> RelatedEntityCueViewModels { get; }

        public DiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode)
              : base(diagram, diagramNode)
        {
            _topLeft = PointExtensions.Undefined;
            _size = Size.Empty;

            DiagramNode = diagramNode;
            DiagramNode.TopLeftChanged += OnTopLeftChanged;

            FocusCommand = new DelegateCommand(Focus);
            DoubleClickCommand = new DelegateCommand(OnDoubleClick);

            RelatedEntityCueViewModels = CreateRelatedEntityCueViewModels();
        }

        public string Name => DiagramNode.Name;
        public string FullName => DiagramNode.FullName;
        public IModelEntity ModelEntity => DiagramNode.ModelEntity;
        public ModelEntityStereotype Stereotype => ModelEntity.Stereotype;
        public bool IsStereotypeVisible => Stereotype != ModelEntityStereotype.None;
        public string StereotypeText => IsStereotypeVisible ? $"<<{Stereotype.Name.ToLower()}>>" : string.Empty;

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

        private void OnTopLeftChanged(IDiagramNode diagramNode, Point2D oldTopLeft, Point2D newTopLeft)
        {
            TopLeft = newTopLeft.ToWpf();
        }

        private void OnSizeChanged(Size newSize)
        {
            DiagramNode.Size = newSize.FromWpf();
        }

        private void OnDoubleClick()
        {
            Diagram.ActivateShape(DiagramNode);
        }

        private List<RelatedEntityCueViewModel> CreateRelatedEntityCueViewModels()
        {
            return Diagram.GetEntityRelationTypes()
                .Select(i => new RelatedEntityCueViewModel(Diagram, DiagramNode, i))
                .ToList();
        }

        public object Clone()
        {
            return new DiagramNodeViewModel(Diagram, DiagramNode)
            {
                _size = _size,
                _topLeft = _topLeft,
            };
        }
    }
}
