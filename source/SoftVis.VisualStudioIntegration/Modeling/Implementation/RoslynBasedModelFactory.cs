using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Creates Roslyn based model instances.
    /// </summary>
    internal class RoslynBasedModelFactory : IImmutableModelFactory
    {
        public ImmutableModel CreateModel() => new RoslynBasedModel();
    }
}
