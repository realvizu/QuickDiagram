namespace Codartis.SoftVis.Util.UI
{
    /// <summary>
    /// A UI element that can be added as a decorator of another (host) UI element.
    /// </summary>
    /// <typeparam name="THost">The type of the UI element that hosts the decorators.</typeparam>
    public interface IUiDecorator<in THost>
    {
        void AssociateWith(THost host);
        void Hide();
    }
}
