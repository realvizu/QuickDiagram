using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines model operations for the application commands package.
    /// </summary>
    public interface IModelServices
    {
        IReadOnlyModel ReadOnlyModel { get; }

        //TODO: ezek helyett builder/factory osztály?
        IRoslynBasedModelEntity GetOrAddRoslynSymbol(INamedTypeSymbol namedTypeSymbol);
        void FindAndAddRelatedEntities(IRoslynBasedModelEntity modelEntity);
    }
}
