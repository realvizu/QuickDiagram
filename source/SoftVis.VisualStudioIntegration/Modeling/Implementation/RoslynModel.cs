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
    internal class RoslynModel : Model
    {
        public RoslynModel()
        {
        }

        private RoslynModel(ModelGraph graph) 
            : base(graph)
        {
        }

        public IEnumerable<RoslynModelNode> RoslynNodes => Nodes.OfType<RoslynModelNode>();

        public IRoslynModelNode GetNodeBySymbol(ISymbol symbol)
        {
            return RoslynNodes.FirstOrDefault(i => i.SymbolEquals(symbol));
        }

        public IRoslynModelNode GetNodeByLocation(Location location)
        {
            if (location == null)
                return null;

            var fileLinePositionSpan = location.GetMappedLineSpan();

            foreach (var roslynModelNode in RoslynNodes)
            {
                var nodeLocation = roslynModelNode.RoslynSymbol?.Locations.FirstOrDefault()?.GetMappedLineSpan();
                if (nodeLocation != null && nodeLocation.Value.Overlaps(fileLinePositionSpan))
                    return roslynModelNode;
            }

            return null;
        }

        public IModelRelationship GetRelationship(IRoslynModelNode sourceNode, IRoslynModelNode targetNode, ModelRelationshipStereotype stereotype)
        {
            return Relationships.FirstOrDefault(i => i.Source == sourceNode && i.Target == targetNode && i.Stereotype == stereotype);
        }

        protected override IModel CreateInstance(ModelGraph graph) => new RoslynModel(graph);
    }
}
