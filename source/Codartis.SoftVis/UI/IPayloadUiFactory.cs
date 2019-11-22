namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates UI objects from payload objects.
    /// </summary>
    public interface IPayloadUiFactory
    {
        IPayloadUi Create(object payload);
    }
}
