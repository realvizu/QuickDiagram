using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Translates from Roslyn concepts to model concepts.
    /// </summary>
    public interface IRoslynSymbolTranslator
    {
        /// <summary>
        /// Controls whether trivial types like object can be added to the model.
        /// </summary>
        bool ExcludeTrivialTypes { get; set; }

        /// <summary>
        /// Returns a value indicating whether the given symbol can be translated to model concepts.
        /// </summary>
        bool IsModeledSymbol([NotNull] ISymbol symbol);

        /// <summary>
        /// Returns the stereotype for a given Roslyn symbol.
        /// </summary>
        ModelNodeStereotype GetStereotype([NotNull] ISymbol symbol);
    }
}