using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Stores the latest roslyn model, provides mutator operations and publishes change events.
    /// </summary>
    internal class RoslynModelStore : ModelStore
    {
        internal RoslynModelStore()
            : base(new RoslynModel())
        {
        }

        public RoslynModel CurrentRoslynModel => (RoslynModel)CurrentModel;
    }
}