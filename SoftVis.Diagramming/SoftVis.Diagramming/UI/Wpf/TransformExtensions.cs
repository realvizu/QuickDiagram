using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf
{
    public static class TransformExtensions
    {
        public static Transform GetInverse(this Transform transform)
        {
            return transform.Inverse as Transform ?? Transform.Identity;
        }
    }
}
