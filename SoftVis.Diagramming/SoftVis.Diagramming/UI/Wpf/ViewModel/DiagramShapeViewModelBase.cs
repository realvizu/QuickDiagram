using System;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for diagram shape view models. Defines position and size.
    /// </summary>
    public abstract class DiagramShapeViewModelBase : DiagramViewModelBase
    {
        private Point _topLeft;
        private Size _size;

        public event Action<IDiagramShape> RemoveRequested;
        public event Action<DiagramShapeViewModelBase> FocusRequested;

        protected DiagramShapeViewModelBase(IArrangedDiagram diagram)
            :base(diagram)
        {
            _topLeft = PointExtensions.Undefined;
            _size = Size.Empty;
        }

        public abstract IDiagramShape DiagramShape { get; }

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
                }
            }
        }

        public void Remove() => RemoveRequested?.Invoke(DiagramShape);
        public void Focus() => FocusRequested?.Invoke(this);

        protected virtual void OnSizeChanged(Size newSize) { }
    }
}
