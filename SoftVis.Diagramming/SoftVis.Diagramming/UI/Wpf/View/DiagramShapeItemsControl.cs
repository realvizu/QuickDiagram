using System.Linq;
using System.Windows;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Presents a collection of diagram shape view models with animated movements, appear and disappear.
    /// </summary>
    internal class DiagramShapeItemsControl : AnimatedItemsControl<DiagramShapeViewModelBase>
    {
        /// <summary>
        /// Returns the control that presents the given diagram shape.
        /// </summary>
        /// <param name="diagramShape">A diagram shape object.</param>
        /// <returns>The control that presents the given diagram shape.</returns>
        public UIElement GetPresenterOf(object diagramShape)
        {
            return this.
                FindChildren<DiagramNodeControl>(i => i.DataContext == diagramShape).FirstOrDefault()?.
                FindParent<AnimatedContentPresenter>();
        }
    }
}
