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
        /// Returns the control that presents the given diagram shape.
        /// </summary>
        /// <param name="diagramShape">A diagram shape view model.</param>
        /// <returns>The control that presents the given diagram shape.</returns>
        public UIElement GetPresenterOf(DiagramShapeViewModelBase diagramShape)
        {
            return this.
                FindFirstDescendant<ContainerDiagramNodeControl>(i => i.DataContext == diagramShape)?.
                FindAncestor<AnimatedRectContentPresenter>();
        }
    }
}
