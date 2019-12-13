using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Compares roslyn symbols for equality.
    /// </summary>
    /// <remarks>
    /// Taken from:
    /// https://stackoverflow.com/questions/34243031/reliably-compare-type-symbols-itypesymbol-with-roslyn#comment56237332_34243031
    /// </remarks>
    public sealed class SymbolEqualityComparer :
        IEqualityComparer<ISymbol>,
        // Implements IEqualityComparer<object> so it can be used for ModelNode payload comparison where the payload is an object.
        IEqualityComparer<object>
    {
        bool IEqualityComparer<ISymbol>.Equals(ISymbol x, ISymbol y)
        {
            return EqualsCore(x, y);
        }

        int IEqualityComparer<ISymbol>.GetHashCode(ISymbol obj)
        {
            return GetHashCodeCore(obj);
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return EqualsCore(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return GetHashCodeCore(obj);
        }

        private static bool EqualsCore(object x, object y)
        {
            return x == null && y == null ||
                   x?.ToString() == y?.ToString();
        }

        private static int GetHashCodeCore([NotNull] object obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}