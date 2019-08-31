using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services
{
    /// <summary>
    /// Creates model service instances.
    /// </summary>
    public interface IModelServiceFactory
    {
        [NotNull]
        IModelService Create();
    }
}