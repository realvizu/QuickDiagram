using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    public static class RoslynModelExtensions
    {
        [NotNull]
        public static IEnumerable<IRoslynModelNode> GetRoslynNodes([NotNull] this IModel model)
        {
            return model.Nodes.OfType<IRoslynModelNode>();
        }

        public static IRoslynModelNode GetNodeBySymbol([NotNull] this IModel model, ISymbol symbol)
        {
            return model.GetRoslynNodes().FirstOrDefault(i => i.SymbolEquals(symbol));
        }

        public static IRoslynModelNode GetNodeByLocation([NotNull] this IModel model, Location location)
        {
            if (location == null)
                return null;

            var fileLinePositionSpan = location.GetMappedLineSpan();

            foreach (var roslynModelNode in model.GetRoslynNodes())
            {
                var nodeLocation = roslynModelNode.RoslynSymbol?.Locations.FirstOrDefault()?.GetMappedLineSpan();
                if (nodeLocation?.Overlaps(fileLinePositionSpan) == true)
                    return roslynModelNode;
            }

            return null;
        }

        public static bool RelationshipExists(
            [NotNull] this IModel model,
            IRoslynModelNode sourceNode,
            IRoslynModelNode targetNode,
            ModelRelationshipStereotype stereotype)
        {
            return model.GetRelationship(sourceNode, targetNode, stereotype) != null;
        }

        public static IModelRelationship GetRelationship(
            [NotNull] this IModel model,
            IRoslynModelNode sourceNode,
            IRoslynModelNode targetNode,
            ModelRelationshipStereotype stereotype)
        {
            return model.Relationships.FirstOrDefault(i => i.Source == sourceNode && i.Target == targetNode && i.Stereotype == stereotype);
        }
    }
}