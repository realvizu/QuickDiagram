using System.Windows;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Geometry;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button on a diagram shape.
    /// </summary>
    public abstract class DiagramButtonViewModelBase : DiagramViewModelBase
    {
        private readonly double _buttonRadius;
        private readonly RectRelativeLocation _rectRelativeLocation;

        private Size _size;
        private Point _relativeTopLeft;
        private bool _isVisible;
        private bool _isEnabled;

        public DelegateCommand ClickCommand { get; private set; }
        public DiagramShapeViewModelBase AssociatedDiagramShapeViewModel { get; private set; }

        protected DiagramButtonViewModelBase(IModel model, Diagram diagram, double buttonRadius, 
            RectRelativeLocation rectRelativeLocation)
            :base(model, diagram)
        {
            _buttonRadius = buttonRadius;
            _rectRelativeLocation = rectRelativeLocation;

            _size = new Size(buttonRadius * 2, buttonRadius * 2);
            _isVisible = false;
            _isEnabled = true;

            ClickCommand = new DelegateCommand(OnClick);
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
                    OnPropertyChanged("Width");
                    OnPropertyChanged("Height");
                }
            }
        }

        public double Width => Size.Width;
        public double Height => Size.Height;

        public Point RelativeTopLeft
        {
            get { return _relativeTopLeft; }
            set
            {
                if (_relativeTopLeft != value)
                {
                    _relativeTopLeft = value;
                    OnPropertyChanged();
                    OnPropertyChanged("RelativeLeft");
                    OnPropertyChanged("RelativeTop");
                }
            }
        }

        public double RelativeTop => RelativeTopLeft.Y;
        public double RelativeLeft => RelativeTopLeft.X;

        public Rect RelativeRect => new Rect(RelativeTopLeft, Size);

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual void AssociateWith(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            AssociatedDiagramShapeViewModel = diagramShapeViewModel;
            RelativeTopLeft = CalculateTopLeft(diagramShapeViewModel);
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
            AssociatedDiagramShapeViewModel = null;
        }

        protected abstract void OnClick();

        private Point CalculateTopLeft(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            var parentTopLeftToButtonCenter = GetButtonCenterRelativeToDiagramShape(diagramShapeViewModel.Size);
            var buttonCenterToButtonTopLeft = new Vector(-_buttonRadius, -_buttonRadius);
            var location = parentTopLeftToButtonCenter + buttonCenterToButtonTopLeft;
            return location;
        }

        private Point GetButtonCenterRelativeToDiagramShape(Size diagramShapeSize)
        {
            return new Rect(diagramShapeSize).GetRelativePoint(_rectRelativeLocation);
        }
    }
}
