using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    internal static class ModelNodeExtensions
    {
        public static ISymbol GetRoslynSymbol([NotNull] this IModelNode modelNode)
        {
            return ((RoslynSymbolBase)modelNode.Payload).UnderlyingSymbol;
        }
    }
}
