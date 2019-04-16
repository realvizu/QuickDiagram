using System.Windows.Media;

namespace Codartis.Util.UI.Wpf
{
    public static class ColorExtensions
    {
        public static Brush CreateBrushFrozen(this Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }
}
