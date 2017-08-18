using Codartis.SoftVis.Modeling.Implementation;

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
