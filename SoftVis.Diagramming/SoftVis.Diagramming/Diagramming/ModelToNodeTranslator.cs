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
            return new ClassNode(item, item.Name, DiagramPoint.Zero, new DiagramSize(100, 25));
        }

        public override DiagramNode Visit(UmlInterface item)
        {
            return new InterfaceNode(item, item.Name, DiagramPoint.Zero, new DiagramSize(100, 35));
        }
    }
}
