using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A widget on a diagram shape.
    /// </summary>
    public abstract class DiagramShapeDecoratorViewModelBase : DiagramViewModelBase
    {
        protected readonly RectRelativeLocation RectRelativeLocation;

        private Size _size;
        private Point _relativeTopLeft;
        private bool _isVisible;

        protected DiagramShapeDecoratorViewModelBase(IReadOnlyModel model, IDiagram diagram,
            double width, double height, RectRelativeLocation rectRelativeLocation)
            : base(model, diagram)
        {
            RectRelativeLocation = rectRelativeLocation;
            _size = new Size(width, height);
            _isVisible = false;
        }

        protected abstract Size ParentSize { get; }

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

        public virtual void Hide()
        {
            IsVisible = false;
        }

        protected virtual Point CalculateTopLeft()
        {
            var widgetCenterRelativeToParent = new Rect(ParentSize).GetRelativePoint(RectRelativeLocation);
            var widgetCenterToWidgetTopLeft = new Vector(-Width / 2, -Height / 2);
            var location = widgetCenterRelativeToParent + widgetCenterToWidgetTopLeft;
            return location;
        }
    }
}