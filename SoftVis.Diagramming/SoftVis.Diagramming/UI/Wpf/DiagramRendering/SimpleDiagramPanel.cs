using System.Linq;
using System.Windows;
using Codartis.SoftVis.UI.Wpf.DiagramRendering.Shapes;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering
{
    /// <summary>
    /// A diagram panel for exporting diagram images.
    /// No pan and zoom support, just the very basic arrangement of the shapes.
    /// </summary>
    internal class SimpleDiagramPanel : DiagramPanelBase
    {
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(SimpleDiagramPanel),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, FontSizeChanged));

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        private static void FontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramPanel = (SimpleDiagramPanel)obj;
            var fontSize = (double)e.NewValue;
            diagramPanel.SetFontSize(fontSize);
        }

        private void SetFontSize(double fontSize)
        {
            foreach (var control in ControlToDiagramShapeMap.Keys)
                control.FontSize = fontSize;
        }

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
