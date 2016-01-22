using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Defines properties for diagram visual elements.
    /// </summary>
    public class DiagramVisual : DependencyObject
    {
        private static readonly Brush FillDefault = Brushes.White;
        private static readonly Brush StrokeDefault = Brushes.Black;

        public static readonly DependencyProperty DiagramFillProperty =
            DependencyProperty.RegisterAttached("DiagramFill", typeof(Brush), typeof(DiagramVisual),
                new FrameworkPropertyMetadata(FillDefault,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DependencyProperty.RegisterAttached("DiagramStroke", typeof(Brush), typeof(DiagramVisual),
                new FrameworkPropertyMetadata(StrokeDefault,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.Inherits));

        public static void SetDiagramFill(UIElement element, Brush value) => element.SetValue(DiagramFillProperty, value);
        public static Brush GetDiagramFill(UIElement element) => (Brush)element.GetValue(DiagramFillProperty);

        public static void SetDiagramStroke(UIElement element, Brush value) => element.SetValue(DiagramStrokeProperty, value);
        public static Brush GetDiagramStroke(UIElement element) => (Brush)element.GetValue(DiagramStrokeProperty);

        public Brush DiagramFill
        {
            get { return (Brush)GetValue(DiagramFillProperty); }
            set { SetValue(DiagramFillProperty, value); }
        }

        public Brush DiagramStroke
        {
            get { return (Brush)GetValue(DiagramStrokeProperty); }
            set { SetValue(DiagramStrokeProperty, value); }
        }
    }
}
