using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model node that represents a Roslyn symbol.
    /// </summary>
    public interface IRoslynModelNode : IModelNode
    {
        /// <summary>
        /// The source of this model item.
        /// </summary>
        ModelOrigin Origin { get; }

        /// <summary>
        /// The Roslyn symbol associated with this node.
        /// </summary>
        ISymbol RoslynSymbol { get; }

        /// <summary>
        /// Returns a value indicating whether this node represents the same roslyn symbol as the one given as parameter,
        /// </summary>
        bool SymbolEquals(ISymbol roslynSymbol);

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="directedModelRelationshipType">
        /// Optionally specifies what kind of directed relationship types should be navigated to find related symbols.
        /// Null means all.
        /// </param>
        /// <returns>Related Roslyn symbols.</returns>
        Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(IRoslynModelProvider roslynModelProvider, 
            DirectedModelRelationshipType? directedModelRelationshipType = null);

        IRoslynModelNode UpdateRoslynSymbol(ISymbol newSymbol);
    }
}