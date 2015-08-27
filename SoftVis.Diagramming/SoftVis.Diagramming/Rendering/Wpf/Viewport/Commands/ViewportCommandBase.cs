using Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Commands
{
    public abstract class ViewportCommandBase
    {
        public object Sender { get; private set; }

        protected ViewportCommandBase(object sender)
        {
            Sender = sender;
        }

        internal abstract void Execute(IViewport viewport);
    }
}
