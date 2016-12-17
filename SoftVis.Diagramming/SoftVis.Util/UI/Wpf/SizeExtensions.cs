using System.Windows;

namespace Codartis.SoftVis.Util.UI.Wpf
{
    public static class SizeExtensions
    {
        public static Size Zero = new Size(0, 0);

        public static bool IsZero(this Size size)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return size.Height == 0 || size.Width == 0;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

    }
}