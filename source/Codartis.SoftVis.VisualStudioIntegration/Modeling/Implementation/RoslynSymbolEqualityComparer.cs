using System.Collections.Generic;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Compares roslyn symbols for equality.
    /// </summary>
    /// <remarks>
    /// Taken from:
    /// https://stackoverflow.com/questions/34243031/reliably-compare-type-symbols-itypesymbol-with-roslyn#comment56237332_34243031
    /// </remarks>
    public sealed class RoslynSymbolEqualityComparer : IEqualityComparer<object>
    {
        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return x?.ToString() == y?.ToString();
        }

        int IEqualityComparer<object>.GetHashCode(object obj) => obj.ToString().GetHashCode();
    }
}