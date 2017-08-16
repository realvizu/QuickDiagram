using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling2.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model created from Roslyn symbols. Immutable.
    /// </summary>
    internal class RoslynBasedModel : ImmutableModel
    {
        public IEnumerable<RoslynModelNode> RoslynModelNodes => Nodes.OfType<RoslynModelNode>();
        public IEnumerable<IRoslynRelationship> RoslynRelationships => Relationships.OfType<IRoslynRelationship>();

        public IRoslynModelNode GetNodeBySymbol(ISymbol symbol)
        {
            return RoslynModelNodes.FirstOrDefault(i => i.SymbolEquals(symbol));
        }

        public IRoslynModelNode GetNodeByLocation(Location location)
        {
            if (location == null)
                return null;

            var fileLinePositionSpan = location.GetMappedLineSpan();

            foreach (var roslynModelNode in RoslynModelNodes)
            {
                var nodeLocation = roslynModelNode.RoslynSymbol?.Locations.FirstOrDefault()?.GetMappedLineSpan();
                if (nodeLocation != null && nodeLocation.Value.Overlaps(fileLinePositionSpan))
                    return roslynModelNode;
            }

            return null;
        }

        public IRoslynRelationship GetRelationship(IRoslynModelNode sourceNode, IRoslynModelNode targetNode, Type type)
        {
            return Relationships.OfType<IRoslynRelationship>().
                FirstOrDefault(i => i.Source == sourceNode && i.Target == targetNode && i.GetType() == type);
        }
    }
}
