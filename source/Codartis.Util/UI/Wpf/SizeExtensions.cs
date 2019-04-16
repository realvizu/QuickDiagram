using System.Windows;

namespace Codartis.Util.UI.Wpf
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

        public static bool IsUndefined(this Size size)
        {
            return double.IsNaN(size.Width) 
                || double.IsNaN(size.Height) 
                || double.IsInfinity(size.Width) 
                || double.IsInfinity(size.Height);
        }

        public static bool IsDefined(this Size size) => !size.IsUndefined();
    }
}