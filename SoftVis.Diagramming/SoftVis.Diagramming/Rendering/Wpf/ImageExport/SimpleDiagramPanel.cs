using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramFixtures;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.ImageExport
{
    public class SimpleDiagramPanel : DiagramPanelBase
    {
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    base.MeasureOverride(availableSize);

        //    foreach (var child in Children.OfType<DiagramShapeControlBase>())
        //        child.Measure(child.Rect.Size);

        //    var desiredSize = Children.OfType<DiagramShapeControlBase>().Select(i => i.Rect).Union().Size;
        //    return desiredSize;
        //}

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);

            foreach (var child in Children.OfType<DiagramShapeControlBase>())
            {
                child.Update();
                child.Arrange(new Rect(child.Rect.X - DiagramRect.X, child.Rect.Y - DiagramRect.Y, child.Rect.Width, child.Rect.Height));
            }

            return arrangeSize;
        }
    }
}
