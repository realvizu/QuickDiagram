using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.Animations;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A WPF Transform bundled together with a hint about its animation.
    /// </summary>
    public struct HintedTransform
    {
        public static readonly HintedTransform Identity = new HintedTransform(Transform.Identity, AnimationHint.None);

        public Transform Transform { get; }
        public AnimationHint AnimationHint { get; }

        public HintedTransform(Transform transform, AnimationHint animationHint = AnimationHint.Short)
        {
            Transform = transform;
            AnimationHint = animationHint;
        }
    }
}
