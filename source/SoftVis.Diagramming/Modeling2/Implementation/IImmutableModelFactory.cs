namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// Creates immutable model instances.
    /// </summary>
    public interface IImmutableModelFactory
    {
        ImmutableModel CreateModel();
    }
}
