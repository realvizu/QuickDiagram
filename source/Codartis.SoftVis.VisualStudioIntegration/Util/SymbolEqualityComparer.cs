using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Util
{
    /// <summary>
    /// Implements an equality comparer for ISymbol objects that compares fully qualified names.
    /// </summary>
    internal class SymbolEqualityComparer : IEqualityComparer<ISymbol>
    {
        public bool Equals(ISymbol symbol1, ISymbol symbol2) => symbol1.SymbolEquals(symbol2);

        public int GetHashCode(ISymbol symbol) => symbol.GetHashCodeForSymbolEquals();
    }
}
