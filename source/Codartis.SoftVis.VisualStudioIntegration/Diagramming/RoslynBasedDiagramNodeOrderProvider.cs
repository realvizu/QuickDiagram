using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    public sealed class RoslynBasedDiagramNodeOrderProvider : IDiagramNodeOrderProvider
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

        public IEnumerable<IDiagramNode> OrderNodes(IEnumerable<IDiagramNode> diagramNodes)
        {
            return diagramNodes
                .OrderBy(i => GetPriority(i.ModelNode.Stereotype))
                .ThenBy(i => i.Name);
        }

        private static int GetPriority(ModelNodeStereotype modelNodeStereotype)
        {
            return ModelNodeStereotypePriorityMap.TryGetValue(modelNodeStereotype, out var priority)
                ? priority
                : 100;
        }
    }
}