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
            var miniButtons = GetMiniButtonsForDiagramNodeControl(diagramNodeControl);

            var hitMiniButton = _hitTester.HitTest<DiagramButton>(e);
            if (!miniButtons.Contains(hitMiniButton))
                Unfocus(diagramNodeControl);
        }

        public void OnMiniButtonMouseEnter(DiagramButton miniButton, MouseEventArgs e)
        {
            var diagramNodeControl = GetDiagramNodeControlForMiniButton(miniButton);
            Focus(diagramNodeControl);
        }

        public void OnMiniButtonMouseLeave(DiagramButton miniButton, MouseEventArgs e)
        {
            var diagramNodeControl = GetDiagramNodeControlForMiniButton(miniButton);
            if (diagramNodeControl == null)
                return;

            var hitDiagramNodeControl = _hitTester.HitTest<DiagramNodeControl2>(e);
            if (diagramNodeControl != hitDiagramNodeControl)
                Unfocus(diagramNodeControl);
        }

        private DiagramNodeControl2 GetDiagramNodeControlForMiniButton(DiagramButton miniButton)
        {
            var miniButtonViewModel = miniButton.DataContext as DiagramButtonViewModelBase;

            var diagramNodeControls = _parentControl.FindChildren<DiagramNodeControl2>(
                i => i.DataContext == miniButtonViewModel?.AssociatedDiagramShapeViewModel);

            return diagramNodeControls.FirstOrDefault();
        }

        private IEnumerable<DiagramButton> GetMiniButtonsForDiagramNodeControl(DiagramNodeControl2 diagramNodeControl)
        {
            var diagramShapeViewModel = diagramNodeControl.DataContext as DiagramShapeViewModelBase;

            var miniButtons = _parentControl.FindChildren<DiagramButton>(
                i => ((DiagramButtonViewModelBase)i.DataContext).AssociatedDiagramShapeViewModel == diagramShapeViewModel);

            return miniButtons;
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
