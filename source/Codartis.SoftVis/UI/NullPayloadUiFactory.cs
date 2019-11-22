namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// A dummy implementation of a payload UI factory.
    /// </summary>
    public sealed class NullPayloadUiFactory : IPayloadUiFactory
    {
        public IPayloadUi Create(object payload) => null;
    }
}
