using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button on a diagram shape.
    /// </summary>
    public abstract class MiniButtonViewModelBase : DiagramShapeDecoratorViewModelBase, IMiniButton
    {
        private bool _isEnabled;
        private string _name;

        public DelegateCommand ClickCommand { get; }
        public DelegateCommand DoubleClickCommand { get; }

        protected MiniButtonViewModelBase(IModelService modelService, IDiagramService diagramService, string name)
            : base(modelService, diagramService)
        {
            _isEnabled = true;
            _name = name;
            ClickCommand = new DelegateCommand(OnClick);
            DoubleClickCommand = new DelegateCommand(OnDoubleClick);
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

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        protected abstract void OnClick();
        protected virtual void OnDoubleClick() { }
    }
}
