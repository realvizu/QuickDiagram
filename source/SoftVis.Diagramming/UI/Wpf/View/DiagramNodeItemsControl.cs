using System.Windows;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Presents a collection of diagram node view models with animated movements, appear and disappear.
    /// </summary>
    internal class DiagramNodeItemsControl : AnimatedItemsControl<DiagramNodeViewModelBase, AnimatedRectContentPresenter>
    {
        /// <summary>
        /// Returns the control that presents the given diagram node.
        /// </summary>
        /// <param name="diagramNode">A diagram node object.</param>
        /// <returns>The control that presents the given diagram node.</returns>
        public UIElement GetPresenterOf(DiagramNodeViewModelBase diagramNode)
        {
            return this.
                FindFirstDescendant<DiagramNodeControl>(i => i.DataContext == diagramNode)?.
                FindAncestor<AnimatedRectContentPresenter>();
        }
    }
}
