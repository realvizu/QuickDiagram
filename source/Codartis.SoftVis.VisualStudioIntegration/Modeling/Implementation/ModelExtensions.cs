using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    public static class ModelExtensions
    {
        public static IEnumerable<RoslynNode> GetRoslynNodes(this IModel model)
        {
            return model.Nodes.Select(i => new RoslynNode(i.Id, i.GetRoslynSymbol()));
        }

        public struct RoslynNode
        {
            public ModelNodeId Id { get; }
            public ISymbol RoslynSymbol { get; }

            public RoslynNode(ModelNodeId id, ISymbol roslynSymbol)
            {
                Id = id;
                RoslynSymbol = roslynSymbol;
            }
        }
    }
}
