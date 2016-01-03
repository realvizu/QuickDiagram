using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines model operations for the application commands package.
    /// </summary>
    public interface IModelServices
    {
        IModel Model { get; }
        IModelEntity GetModelEntity(ISymbol symbol);
    }
}
