namespace Codartis.Util.UI
{
    /// <summary>
    /// A UI element that can be added to another UI element (the host).
    /// </summary>
    /// <typeparam name="THost">The type of the UI element that hosts the decorators.</typeparam>
    public interface IUiDecorator<THost>
    {
        THost HostUiElement { get; }

        void AssociateWith(THost host);
        
        void Hide();
    }
}