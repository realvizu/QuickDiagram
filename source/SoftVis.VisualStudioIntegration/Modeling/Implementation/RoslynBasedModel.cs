using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model created from Roslyn symbols. Immutable.
    /// </summary>
    internal class RoslynBasedModel : ImmutableModel
    {
        public RoslynBasedModel()
        {
        }

        private RoslynBasedModel(ImmutableModelGraph graph) 
            : base(graph)
        {
        }

        public IEnumerable<RoslynModelNode> RoslynModelNodes => Nodes.OfType<RoslynModelNode>();
        public IEnumerable<ModelRelationshipBase> RoslynRelationships => Relationships.OfType<ModelRelationshipBase>();

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

        public IModelRelationship GetRelationship(IRoslynModelNode sourceNode, IRoslynModelNode targetNode, ModelRelationshipStereotype stereotype)
        {
            return RoslynRelationships.FirstOrDefault(i => i.Source == sourceNode && i.Target == targetNode && i.Stereotype == stereotype);
        }

        protected override ImmutableModel CreateClone(ImmutableModelGraph graph) =>
            new RoslynBasedModel(graph);
    }
}
