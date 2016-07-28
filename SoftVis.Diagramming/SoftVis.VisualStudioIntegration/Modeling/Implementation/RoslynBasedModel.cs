using System.Linq;
using Codartis.SoftVis.Modeling.Implementation;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model created from Roslyn symbols.
    /// </summary>
    internal class RoslynBasedModel : Model
    {
        public RoslynBasedModelEntity GetModelEntity(INamedTypeSymbol namedTypeSymbol)
        {
            return Entities.OfType<RoslynBasedModelEntity>()
                .FirstOrDefault(i => Equals(i.RoslynSymbol, namedTypeSymbol));
        }
    }
}
