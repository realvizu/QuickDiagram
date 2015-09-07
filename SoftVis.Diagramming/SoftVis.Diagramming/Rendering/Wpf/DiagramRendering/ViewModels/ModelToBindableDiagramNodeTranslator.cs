using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels
{
    class ModelToBindableDiagramNodeTranslator : UmlModelVisitorBase<DiagramNode>
    {
        internal static DiagramNode Translate(UmlTypeOrPackage typeOrPackage)
        {
            var visitor = new ModelToBindableDiagramNodeTranslator();
            var node = visitor.Visit(typeOrPackage);
            return node;
        }

        public override DiagramNode Visit(UmlClass item)
        {
            return new BindableClassNode(item, item.Name, DiagramPoint.Zero, new DiagramSize(100, 25));
        }
    }
}
