using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A specialization of the model entity concept that references a Roslyn symbol.
    /// Capable of finding related Roslyn symbols in the Roslyn API.
    /// </summary>
    public interface IRoslynBasedModelEntity : IModelEntity
    {
        /// <summary>
        /// Gets the Roslyn symbol associated with this entity.
        /// </summary>
        INamedTypeSymbol RoslynSymbol { get; set; }

        /// <summary>
        /// Returns a value indicating whether this entity represents the same roslyn symbol as the one given as parameter,
        /// </summary>
        bool SymbolEquals(INamedTypeSymbol roslynSymbol);

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="entityRelationType">Optionally specifies what kind of relations should be found. Null means all relations.</param>
        /// <returns>Related Roslyn symbols.</returns>
        Task<IEnumerable<RoslynSymbolRelation>> FindRelatedSymbolsAsync(IRoslynModelProvider roslynModelProvider, 
            EntityRelationType? entityRelationType = null);
    }
}