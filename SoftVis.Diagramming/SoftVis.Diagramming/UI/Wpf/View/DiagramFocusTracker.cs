using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Codartis.SoftVis.UI.Wpf.Common;
using Codartis.SoftVis.UI.Wpf.Common.HitTesting;
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
        private DiagramNodeControl2 _focusOwner;

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

        private DiagramNodeControl2 FindPointedControl(MouseEventArgs mouseEventArgs)
        {
            var pointedDiagramButton = _hitTester.HitTest<DiagramButton>(mouseEventArgs);
            if (pointedDiagramButton != null)
                return GetDiagramNodeControlForButton(pointedDiagramButton);

            var pointedDiagramNode = _hitTester.HitTest<DiagramNodeControl2>(mouseEventArgs);
            if (pointedDiagramNode != null)
                return pointedDiagramNode;

            return null;
        }

        private DiagramNodeControl2 GetDiagramNodeControlForButton(DiagramButton diagramButton)
        {
            var diagramButtonViewModel = diagramButton.DataContext as DiagramButtonViewModelBase;

            var diagramNodeControls = _parentControl.FindChildren<DiagramNodeControl2>(
                i => i.DataContext == diagramButtonViewModel?.AssociatedDiagramShapeViewModel);

            return diagramNodeControls.FirstOrDefault();
        }

        private static void Focus(DiagramNodeControl2 diagramNodeControl)
        {
            diagramNodeControl?.FocusCommand?.Execute();
        }

        private static void Unfocus(DiagramNodeControl2 diagramNodeControl)
        {
            diagramNodeControl?.UnfocusCommand?.Execute();
        }
    }
}
