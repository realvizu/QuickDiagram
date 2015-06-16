using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    class ModelToNodeTranslator : UmlModelVisitorBase<DiagramNode>
    {
        internal static DiagramNode Translate(UmlTypeOrPackage typeOrPackage)
        {
            var visitor = new ModelToNodeTranslator();
            var node = visitor.Visit(typeOrPackage);
            return node;
        }

        public override DiagramNode Visit(UmlClass item)
        {
            var node = new ClassNode
            {
                ModelElement = item,
                Name = item.Name,
                Size = new DiagramSize(100, 25),
                Position = DiagramPoint.Zero,
            };
            return node;
        }

        public override DiagramNode Visit(UmlInterface item)
        {
            var node = new InterfaceNode
            {
                ModelElement = item,
                Name = item.Name,
                Size = new DiagramSize(100, 35),
                Position = DiagramPoint.Zero,
            };
            return node;
        }
    }
}
