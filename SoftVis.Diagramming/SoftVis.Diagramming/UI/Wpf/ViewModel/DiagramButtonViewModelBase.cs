using System;
using System.Windows;
using Codartis.SoftVis.UI.Geometry;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button on a diagram shape.
    /// </summary>
    public abstract class DiagramButtonViewModelBase : ViewModelBase
    {
        private readonly double _buttonRadius;
        private readonly RectRelativeLocation _rectRelativeLocation;
        private readonly Action<DiagramShapeViewModelBase> _clickCommandDelegate;

        private Size _size;
        private Point _topLeft;
        private bool _isVisible;
        private bool _isEnabled;

        public ParameterlessCommand ClickCommand { get; private set; }
        public DiagramShapeViewModelBase AssociatedDiagramShapeViewModel { get; private set; }

        protected DiagramButtonViewModelBase(double buttonRadius, RectRelativeLocation rectRelativeLocation,
            Action<DiagramShapeViewModelBase> clickCommandDelegate)
        {
            _buttonRadius = buttonRadius;
            _rectRelativeLocation = rectRelativeLocation;
            _clickCommandDelegate = clickCommandDelegate;

            _size = new Size(buttonRadius * 2, buttonRadius * 2);
            _isVisible = false;
            _isEnabled = true;

            ClickCommand = new ParameterlessCommand(OnClick);
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

        public virtual void AssociateWith(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            AssociatedDiagramShapeViewModel = diagramShapeViewModel;
            TopLeft = CalculateTopLeft(diagramShapeViewModel);
            IsVisible = true;
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
            var buttonCenterToButtonTopLeft = new Vector(-_buttonRadius, -_buttonRadius);
            var location = parentTopLeft + parentTopLeftToButtonCenter + buttonCenterToButtonTopLeft;
            return location;
        }

        private Vector GetButtonCenterRelativeToDiagramShape(Size diagramShapeSize)
        {
            return (Vector)new Rect(diagramShapeSize).GetRelativePoint(_rectRelativeLocation);
        }

        private void OnClick()
        {
            _clickCommandDelegate(AssociatedDiagramShapeViewModel);
        }
    }
}
