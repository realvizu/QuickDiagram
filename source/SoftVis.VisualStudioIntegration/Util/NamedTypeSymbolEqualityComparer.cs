using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Util
{
    /// <summary>
    /// Implements an equality comparer for INamedTypeSymbols that compares fully qualified names.
    /// </summary>
    internal class NamedTypeSymbolEqualityComparer : IEqualityComparer<INamedTypeSymbol>
    {
        public bool Equals(INamedTypeSymbol namedTypeSymbol1, INamedTypeSymbol namedTypeSymbol2) 
            => namedTypeSymbol1.SymbolEquals(namedTypeSymbol2);

        public int GetHashCode(INamedTypeSymbol namedTypeSymbol) 
            => namedTypeSymbol.GetHashCodeForSymbolEquals();
    }
}
