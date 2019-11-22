using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A widget on a diagram shape. 
    /// Its placement is calculated by the view using the PlacementKey.
    /// </summary>
    public abstract class DiagramShapeDecoratorViewModelBase : ModelObserverViewModelBase, IUiDecorator<IDiagramShapeUi>
    {
        private bool _isVisible;

        public IDiagramShapeUi HostUiElement { get; private set; }

        protected DiagramShapeDecoratorViewModelBase(
            IModelEventSource modelEventSource,
            IDiagramEventSource diagramEventSource)
            : base(modelEventSource, diagramEventSource)
        {
            _isVisible = false;
        }

        /// <summary>
        /// An object that serves as the key when the view looks up the placement specification in a dictionary.
        /// Its value is dependent on which kind of diagram shape button it is.
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

        public virtual void AssociateWith(IDiagramShapeUi host)
        {
            HostUiElement = host;
            IsVisible = true;
        }

        public virtual void Hide()
        {
            HostUiElement = null;
            IsVisible = false;
        }
    }
}