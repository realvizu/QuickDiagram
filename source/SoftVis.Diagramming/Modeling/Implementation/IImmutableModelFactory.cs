namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Creates immutable model instances.
    /// </summary>
    public interface IImmutableModelFactory
    {
        ImmutableModel CreateModel();
    }
}
