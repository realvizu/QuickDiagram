using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A minibutton is a button on a diagram shape.
    /// </summary>
    public abstract class MiniButtonViewModelBase : ViewModelBase
    {
        private RectRelativeLocation _relativeLocation;
        private Size _size;
        private Point _topLeft;
        private bool _isVisible;
        private bool _isEnabled;
        private ICommand _buttonClickedCommand;

        public DiagramShapeViewModelBase AssociatedDiagramShapeViewModel { get; private set; }

        protected MiniButtonViewModelBase(double miniButtonRadius)
        {
            _size = new Size(miniButtonRadius*2, miniButtonRadius*2);
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

        public double Left => TopLeft.X;
        public double Top => TopLeft.Y;

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
            TopLeft = (Point) (diagramShapeViewModel.Position - new Point(5d, 5d));
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }
    }
}
