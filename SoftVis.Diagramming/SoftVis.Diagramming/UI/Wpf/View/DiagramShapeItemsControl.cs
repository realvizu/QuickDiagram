using System.Windows;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Presents a collection of diagram shape view models with animated movements, appear and disappear.
    /// </summary>
    internal class DiagramShapeItemsControl : AnimatedItemsControl<DiagramShapeViewModelBase, DiagramShapeItemPresenter>
    {
        /// <summary>
        /// Returns the control that presents the given diagram shape.
        /// </summary>
        /// <param name="diagramShape">A diagram shape object.</param>
        /// <returns>The control that presents the given diagram shape.</returns>
        public UIElement GetPresenterOf(DiagramShapeViewModelBase diagramShape)
        {
            return this.
                FindFirstDescendant<DiagramNodeControl>(i => i.DataContext == diagramShape)?.
                FindAncestor<DiagramShapeItemPresenter>();
        }
    }
}
