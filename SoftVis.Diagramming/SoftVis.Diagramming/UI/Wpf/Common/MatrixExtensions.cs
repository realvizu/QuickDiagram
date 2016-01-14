using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.Common
{
    public static class MatrixExtensions
    {
        public static string ToDebugString(this Matrix matrix)
        {
            return $"{matrix.M11:0.00} | {matrix.OffsetX:0.00} | {matrix.OffsetY:0.00}";
        }
    }
}
