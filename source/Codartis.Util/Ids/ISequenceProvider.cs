namespace Codartis.Util.Ids
{
    /// <summary>
    /// Provides unique values.
    /// </summary>
    public interface ISequenceProvider
    {
        long GetNext();
    }
}
