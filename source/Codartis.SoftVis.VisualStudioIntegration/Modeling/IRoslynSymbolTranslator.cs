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

        bool IsModeledType([NotNull] ISymbol symbol);

        bool IsModeledMember([NotNull] ISymbol symbol);

        /// <summary>
        /// Returns a value indicating whether the given symbol is a member that defines an association relationship.
        /// (Eg. fields, properties, etc.)
        /// </summary>
        bool IsAssociationMember(ISymbol symbol);

        /// <summary>
        /// If the given symbol is a member that has a type then gives back that type's symbol, otherwise throws.
        /// </summary>
        ISymbol GetTypeSymbolOfMemberSymbol(ISymbol symbol);

        ModelNodeStereotype GetStereotype([NotNull] ISymbol symbol);

        ModelOrigin GetOrigin([NotNull] ISymbol symbol);

        [NotNull]
        string GetName([NotNull] ISymbol symbol);

        [NotNull]
        string GetFullName([NotNull] ISymbol symbol);

        [NotNull]
        string GetDescription([NotNull] ISymbol symbol);

        bool GetIsStatic([NotNull] ISymbol symbol);
        
        bool GetIsAbstract([NotNull] ISymbol symbol);
    }
}