using System.Windows.Media;

namespace Codartis.SoftVis.Util.UI.Wpf.Transforms
{
    public static class TransformExtensions
    {
        public static Transform GetInverse(this Transform transform)
        {
            return transform.Inverse as Transform ?? Transform.Identity;
        }
    }
}
