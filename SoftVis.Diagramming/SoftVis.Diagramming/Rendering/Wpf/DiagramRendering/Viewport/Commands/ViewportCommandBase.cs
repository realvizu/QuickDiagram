namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands
{
    internal abstract class ViewportCommandBase
    {
        internal object Sender { get; private set; }

        protected ViewportCommandBase(object sender)
        {
            Sender = sender;
        }

        internal abstract void Execute(IDiagramViewport diagramViewport);
    }
}
