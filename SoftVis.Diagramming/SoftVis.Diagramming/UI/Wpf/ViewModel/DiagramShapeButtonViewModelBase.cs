using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Geometry;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button on a diagram shape.
    /// </summary>
    public abstract class DiagramShapeButtonViewModelBase : DiagramShapeDecoratorViewModelBase
    {
        private bool _isEnabled;

        public DelegateCommand ClickCommand { get; }
        public DiagramShapeViewModelBase AssociatedDiagramShapeViewModel { get; private set; }

        protected DiagramShapeButtonViewModelBase(IReadOnlyModel model, IDiagram diagram, double buttonRadius,
            RectRelativeLocation rectRelativeLocation)
            : base(model, diagram, buttonRadius * 2, buttonRadius * 2, rectRelativeLocation)
        {
            _isEnabled = true;
            ClickCommand = new DelegateCommand(OnClick);
        }

        protected override Size ParentSize => AssociatedDiagramShapeViewModel?.Size ?? new Size();

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
            RelativeTopLeft = CalculateTopLeft();
            IsVisible = true;
        }

        protected abstract void OnClick();

        public override void Hide()
        {
            base.Hide();
            AssociatedDiagramShapeViewModel = null;
        }
    }
}
