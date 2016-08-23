using System;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks which diagram node the user points to (focused node) 
    /// and fires events when a node should receive the decorations (visible minibuttons).
    /// The decoration can be pinned meaning that it won't follow the focus until unpinned.
    /// </summary>
    public class DiagramFocusTracker
    {
        /// <summary>The focused shape is the one that the user points to.</summary>
        private DiagramNodeViewModel _focusedDiagramNode;

        /// <summary>The decoration is pinned of the minibuttons stay visible even when focus is lost from a shape.</summary>
        private bool _isDecorationPinned;

        public event Action<DiagramNodeViewModel> DecoratedNodeChanged;

        public DiagramFocusTracker()
        {
            _focusedDiagramNode = null;
            _isDecorationPinned = false;
        }

        public void Focus(DiagramShapeViewModelBase diagramShapeViewModelBase)
        {
            var diagramNodeViewModel = diagramShapeViewModelBase as DiagramNodeViewModel;
            if (diagramNodeViewModel == null)
                return;

            if (diagramNodeViewModel == _focusedDiagramNode)
                return;

            Unfocus(_focusedDiagramNode);
            SetFocus(diagramNodeViewModel);
        }

        public void Unfocus(DiagramShapeViewModelBase diagramShapeViewModelBase)
        {
            var diagramNodeViewModel = diagramShapeViewModelBase as DiagramNodeViewModel;
            if (diagramNodeViewModel == null)
                return;

            if (_focusedDiagramNode == diagramNodeViewModel)
                SetFocus(null);
        }

        public void UnfocusAll()
        {
            if (_focusedDiagramNode != null)
                Unfocus(_focusedDiagramNode);
        }

        public void PinDecoration()
        {
            _isDecorationPinned = true;
        }

        public void UnpinDecoration()
        {
            _isDecorationPinned = false;
            ChangeDecorationTo(_focusedDiagramNode);
        }

        private void SetFocus(DiagramNodeViewModel diagramNodeViewModel)
        {
            _focusedDiagramNode = diagramNodeViewModel;

            if (!_isDecorationPinned)
                ChangeDecorationTo(_focusedDiagramNode);
        }

        private void ChangeDecorationTo(DiagramNodeViewModel diagramNodeViewModel)
        {
            DecoratedNodeChanged?.Invoke(diagramNodeViewModel);
        }
    }
}