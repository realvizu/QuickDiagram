using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Geometry;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button on a diagram shape.
    /// </summary>
    public abstract class DiagramButtonViewModelBase : ViewModelBase
    {
        private Size _size;
        private Point _topLeft;
        private bool _isVisible;
        private bool _isEnabled;
        private ICommand _buttonClickedCommand;

        protected double ButtonRadius { get; }
        protected RectRelativeLocation RectRelativeLocation { get; }

        public DiagramShapeViewModelBase AssociatedDiagramShapeViewModel { get; private set; }

        protected DiagramButtonViewModelBase(double buttonRadius, RectRelativeLocation rectRelativeLocation)
        {
            ButtonRadius = buttonRadius;
            RectRelativeLocation = rectRelativeLocation;

            _size = new Size(buttonRadius*2, buttonRadius*2);
            _isVisible = false;
            _isEnabled = true;
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

        public Point TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (_topLeft != value)
                {
                    _topLeft = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Left");
                    OnPropertyChanged("Top");
                }
            }
        }

        public double Top => TopLeft.Y;
        public double Left => TopLeft.X;

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

        public ICommand ButtonClickedCommand
        {
            get { return _buttonClickedCommand; }
            set
            {
                if (_buttonClickedCommand != value)
                {
                    _buttonClickedCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AssociateWith(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            AssociatedDiagramShapeViewModel = diagramShapeViewModel;
            TopLeft = CalculateTopLeft(diagramShapeViewModel);
            IsVisible = true;
            IsEnabled = ((DiagramNode) diagramShapeViewModel.DiagramShape).Name.StartsWith("1");
        }

        public void Hide()
        {
            AssociatedDiagramShapeViewModel = null;
            IsVisible = false;
        }

        private Point CalculateTopLeft(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            var parentTopLeft = diagramShapeViewModel.Position;
            var parentTopLeftToButtonCenter = GetButtonCenterRelativeToDiagramShape(diagramShapeViewModel.Size);
            var buttonCenterToButtonTopLeft = new Vector(-ButtonRadius, -ButtonRadius);
            var location = parentTopLeft + parentTopLeftToButtonCenter + buttonCenterToButtonTopLeft;
            return location;
        }

        private Vector GetButtonCenterRelativeToDiagramShape(Size diagramShapeSize)
        {
            return (Vector) new Rect(diagramShapeSize).GetRelativePoint(RectRelativeLocation);
        }
    }
}
