using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.Animations
{
    /// <summary>
    /// A WPF Transform bundled together with a hint about its animation.
    /// </summary>
    public struct AnimatedTransform
    {
        public static readonly AnimatedTransform Identity = new AnimatedTransform(Transform.Identity, AnimationHint.None);

        public Transform Transform { get; }
        public AnimationHint AnimationHint { get; }

        public AnimatedTransform(Transform transform, AnimationHint animationHint = AnimationHint.Short)
        {
            Transform = transform;
            AnimationHint = animationHint;
        }
    }
}
