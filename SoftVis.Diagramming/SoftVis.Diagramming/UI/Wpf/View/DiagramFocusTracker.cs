using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Codartis.SoftVis.UI.Wpf.HitTesting;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// UI logic to track diagram shape focus.
    /// </summary>
    internal class DiagramFocusTracker
    {
        private readonly Control _parentControl;
        private readonly HitTester _hitTester;
        private DiagramNodeControl _focusOwner;

        public DiagramFocusTracker(Control parentControl)
        {
            _parentControl = parentControl;
            _hitTester = new HitTester(parentControl);
            _focusOwner = null;
        }

        public void Unfocus()
        {
            if (_focusOwner != null)
            {
                Unfocus(_focusOwner);
                _focusOwner = null;
            }
        }

        public void TrackMouse(MouseEventArgs mouseEventArgs)
        {
            var pointedControl = FindPointedControl(mouseEventArgs);
            if (pointedControl != _focusOwner)
            {
                Unfocus(_focusOwner);
                _focusOwner = pointedControl;
                Focus(_focusOwner);
            }
        }

        private DiagramNodeControl FindPointedControl(MouseEventArgs mouseEventArgs)
        {
            var pointedDiagramButton = _hitTester.HitTest<DiagramButton>(mouseEventArgs);
            return pointedDiagramButton != null 
                ? GetDiagramNodeControlForButton(pointedDiagramButton) 
                : _hitTester.HitTest<DiagramNodeControl>(mouseEventArgs);
        }

        private DiagramNodeControl GetDiagramNodeControlForButton(DiagramButton diagramButton)
        {
            var diagramButtonViewModel = diagramButton.DataContext as DiagramButtonViewModelBase;

            var diagramNodeControls = _parentControl.FindChildren<DiagramNodeControl>(
                i => i.DataContext == diagramButtonViewModel?.AssociatedDiagramShapeViewModel);

            return diagramNodeControls.FirstOrDefault();
        }

        private static void Focus(DiagramNodeControl diagramNodeControl) => diagramNodeControl?.FocusDiagramItem();
        private static void Unfocus(DiagramNodeControl diagramNodeControl) => diagramNodeControl?.UnfocusDiagramItem();
    }
}
