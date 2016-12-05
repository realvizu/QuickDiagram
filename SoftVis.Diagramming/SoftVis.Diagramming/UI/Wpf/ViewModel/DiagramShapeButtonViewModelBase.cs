using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button on a diagram shape.
    /// </summary>
    public abstract class DiagramShapeButtonViewModelBase : DiagramShapeDecoratorViewModelBase, IDisposable
    {
        private bool _isEnabled;

        public DelegateCommand ClickCommand { get; }
        public DelegateCommand DoubleClickCommand { get; }
        public DiagramShapeViewModelBase AssociatedDiagramShapeViewModel { get; private set; }

        protected DiagramShapeButtonViewModelBase(IArrangedDiagram diagram)
            : base(diagram)
        {
            _isEnabled = true;
            ClickCommand = new DelegateCommand(OnClick);
            DoubleClickCommand = new DelegateCommand(OnDoubleClick);
        }

        public virtual void Dispose() { }

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
            IsVisible = true;
        }

        protected abstract void OnClick();
        protected virtual void OnDoubleClick() { }

        public override void Hide()
        {
            base.Hide();
            AssociatedDiagramShapeViewModel = null;
        }
    }
}
