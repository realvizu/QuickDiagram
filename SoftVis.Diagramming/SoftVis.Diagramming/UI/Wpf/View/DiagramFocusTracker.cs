using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.HitTesting;

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
            var diagramButtonViewModel = diagramButton.DataContext as DiagramShapeButtonViewModelBase;
            var associatedDiagramShapeViewModel = diagramButtonViewModel?.AssociatedDiagramShapeViewModel;

            return associatedDiagramShapeViewModel == null
                ? null
                : _parentControl.FindFirstDescendant<DiagramNodeControl>(i => i.DataContext == associatedDiagramShapeViewModel);
        }

        private static void Focus(DiagramNodeControl diagramNodeControl) => diagramNodeControl?.FocusDiagramItem();
        private static void Unfocus(DiagramNodeControl diagramNodeControl) => diagramNodeControl?.UnfocusDiagramItem();
    }
}
