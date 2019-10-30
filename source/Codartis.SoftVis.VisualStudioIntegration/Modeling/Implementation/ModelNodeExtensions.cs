using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
