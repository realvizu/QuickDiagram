using System.Linq;
using System.Windows;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering
{
    public class SimpleDiagramPanel : DiagramPanelBase
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (var child in Children.OfType<DiagramShapeControlBase>())
            {
                var position = (Point)(child.Position - ContentRect.TopLeft);
                child.Arrange(new Rect(position, child.DesiredSize));
            }

            return arrangeSize;
        }
    }
}
