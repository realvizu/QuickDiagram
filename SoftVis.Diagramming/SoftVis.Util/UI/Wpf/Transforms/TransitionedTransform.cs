using System.Windows.Media;

namespace Codartis.SoftVis.Util.UI.Wpf.Transforms
{
    /// <summary>
    /// A WPF Transform bundled together with a transition speed.
    /// </summary>
    public struct TransitionedTransform
    {
        public static readonly TransitionedTransform Identity = 
            new TransitionedTransform(Transform.Identity, TransitionSpeed.Instant);

        public Transform Transform { get; }
        public TransitionSpeed TransitionSpeed { get; }

        public TransitionedTransform(Transform transform, TransitionSpeed transitionSpeed = TransitionSpeed.Fast)
        {
            Transform = transform;
            TransitionSpeed = transitionSpeed;
        }

        public override string ToString()
        {
            return $"[{Transform.Value.OffsetX},{Transform.Value.OffsetY} x{Transform.Value.M11} ({TransitionSpeed})]";
        }
    }
}
