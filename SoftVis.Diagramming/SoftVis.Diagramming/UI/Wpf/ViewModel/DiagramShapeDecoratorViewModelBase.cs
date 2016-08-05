using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A widget on a diagram shape. 
    /// Its placement is calculated by the view using the PlacementKey.
    /// </summary>
    public abstract class DiagramShapeDecoratorViewModelBase : DiagramViewModelBase
    {
        private bool _isVisible;

        protected DiagramShapeDecoratorViewModelBase(IReadOnlyModel model, IDiagram diagram)
            : base(model, diagram)
        {
            _isVisible = false;
        }

        /// <summary>
        /// An object that serves as the key when the view looks up the placement specification in a dictionary.
        /// Its value is dependent on which kind of diagram shape button is it.
        /// </summary>
        public abstract object PlacementKey { get; }

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

        public virtual void Hide() => IsVisible = false;
    }
}