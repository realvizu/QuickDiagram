using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels
{
    public class BindableDiagram : Diagram
    {
        protected override DiagramNode CreateDiagramNode(UmlTypeOrPackage umlTypeOrPackage)
        {
            return ModelToBindableDiagramNodeTranslator.Translate(umlTypeOrPackage);
        }
    }
}
