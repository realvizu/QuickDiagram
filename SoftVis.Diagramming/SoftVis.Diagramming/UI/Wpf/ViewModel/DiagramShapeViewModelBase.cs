using System;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for diagram shape view models. Defines position and size.
    /// </summary>
    public abstract class DiagramShapeViewModelBase : FocusableViewModelBase
    {
        protected readonly IReadOnlyModel Model;
        protected readonly IDiagram Diagram;

        private Point _position;
        private Size _size;

        public event Action<IDiagramShape> RemoveRequested;

        protected DiagramShapeViewModelBase(IReadOnlyModel model, IDiagram diagram)
        {
            Model = model;
            Diagram = diagram;
        }

        public abstract IDiagramShape DiagramShape { get; }

        public Point Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged();
                    OnPropertyChanged("X");
                    OnPropertyChanged("Y");
                }
            }
        }

        public double X => Position.X;
        public double Y => Position.Y;

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

        public abstract void UpdateState();

        public void Remove() => RemoveRequested?.Invoke(DiagramShape);
    }
}
