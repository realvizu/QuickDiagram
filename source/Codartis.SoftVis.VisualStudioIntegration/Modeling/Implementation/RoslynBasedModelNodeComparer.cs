using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    public sealed class RoslynBasedModelNodeComparer : IComparer<IModelNode>
    {
        [NotNull]
        private static readonly IDictionary<ModelNodeStereotype, int> ModelNodeStereotypePriorityMap =
            new Dictionary<ModelNodeStereotype, int>
            {
                [ModelNodeStereotypes.Constant] = 0,
                [ModelNodeStereotypes.Field] = 1,
                [ModelNodeStereotypes.Property] = 2,
                [ModelNodeStereotypes.Event] = 3,
                [ModelNodeStereotypes.Method] = 4,
            };

        public int Compare(IModelNode x, IModelNode y)
        {
            var stereotypeComparisonResult = GetStereotypePriority(x) - GetStereotypePriority(y);
            if (stereotypeComparisonResult != 0)
                return stereotypeComparisonResult;

            var symbolNameX = GetSymbolName(x);
            var symbolNameY = GetSymbolName(y);
            return string.Compare(symbolNameX, symbolNameY, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetSymbolName(IModelNode modelNode)
        {
            var symbol = (ISymbol)modelNode.Payload;
            return symbol.Name;
        }

        private static int GetStereotypePriority([NotNull] IModelNode x)
        {
            return ModelNodeStereotypePriorityMap.TryGetValue(x.Stereotype, out var priority)
                ? priority
                : 100;
        }
    }
}