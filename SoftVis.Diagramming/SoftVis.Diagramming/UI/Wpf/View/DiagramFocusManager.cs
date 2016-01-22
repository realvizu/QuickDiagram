using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Codartis.SoftVis.UI.Wpf.Common;
using Codartis.SoftVis.UI.Wpf.Common.HitTesting;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// UI logic to manage diagram shape focus.
    /// </summary>
    internal class DiagramFocusManager
    {
        private readonly Control _parentControl;
        private readonly HitTester _hitTester;

        public DiagramFocusManager(Control parentControl)
        {
            _parentControl = parentControl;
            _hitTester = new HitTester(parentControl);
        }

        public void UnfocusAll()
        {
            var diagramNodeControls = _parentControl.FindChildren<DiagramNodeControl2>();
            foreach (var diagramNodeControl in diagramNodeControls)
                Unfocus(diagramNodeControl);
        }

        public void OnDiagramNodeMouseEnter(DiagramNodeControl2 diagramNodeControl, MouseEventArgs e)
        {
            Focus(diagramNodeControl);
        }

        public void OnDiagramNodeMouseLeave(DiagramNodeControl2 diagramNodeControl, MouseEventArgs e)
        {
            var diagramButtons = GetButtonsForDiagramNodeControl(diagramNodeControl);

            var diagramButtonHit = _hitTester.HitTest<DiagramButton>(e);
            if (!diagramButtons.Contains(diagramButtonHit))
                Unfocus(diagramNodeControl);
        }

        public void OnDiagramButtonMouseEnter(DiagramButton diagramButton, MouseEventArgs e)
        {
            var diagramNodeControl = GetDiagramNodeControlForButton(diagramButton);
            Focus(diagramNodeControl);
        }

        public void OnDiagramButtonMouseLeave(DiagramButton diagramButton, MouseEventArgs e)
        {
            var diagramNodeControl = GetDiagramNodeControlForButton(diagramButton);
            if (diagramNodeControl == null)
                return;

            var diagramNodeControlHit = _hitTester.HitTest<DiagramNodeControl2>(e);
            if (diagramNodeControl != diagramNodeControlHit)
                Unfocus(diagramNodeControl);
        }

        private DiagramNodeControl2 GetDiagramNodeControlForButton(DiagramButton diagramButton)
        {
            var diagramButtonViewModel = diagramButton.DataContext as DiagramButtonViewModelBase;

            var diagramNodeControls = _parentControl.FindChildren<DiagramNodeControl2>(
                i => i.DataContext == diagramButtonViewModel?.AssociatedDiagramShapeViewModel);

            return diagramNodeControls.FirstOrDefault();
        }

        private IEnumerable<DiagramButton> GetButtonsForDiagramNodeControl(DiagramNodeControl2 diagramNodeControl)
        {
            var diagramShapeViewModel = diagramNodeControl.DataContext as DiagramShapeViewModelBase;

            var diagramButtons = _parentControl.FindChildren<DiagramButton>(
                i => ((DiagramButtonViewModelBase)i.DataContext).AssociatedDiagramShapeViewModel == diagramShapeViewModel);

            return diagramButtons;
        }

        private static void Focus(DiagramNodeControl2 diagramNodeControl)
        {
            diagramNodeControl?.FocusCommand?.Execute(null);
        }

        private static void Unfocus(DiagramNodeControl2 diagramNodeControl)
        {
            diagramNodeControl?.UnfocusCommand?.Execute(null);
        }
    }
}
