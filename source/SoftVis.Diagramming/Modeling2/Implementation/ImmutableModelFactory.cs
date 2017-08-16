namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// The default implementation of IImmutableModelFactory.
    /// </summary>
    public class ImmutableModelFactory : IImmutableModelFactory
    {
        public ImmutableModel CreateModel() => new ImmutableModel();
    }
}
