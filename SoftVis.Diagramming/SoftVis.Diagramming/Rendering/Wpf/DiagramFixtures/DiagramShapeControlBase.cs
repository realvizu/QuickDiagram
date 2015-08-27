using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramFixtures
{
    public abstract class DiagramShapeControlBase : Control
    {
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register("Rect", typeof(Rect), typeof(DiagramShapeControlBase),
                new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        public abstract void Update();
    }
}
