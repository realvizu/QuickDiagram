using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Shapes;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering
{
    /// <summary>
    /// A diagram that can be rendered with WPF.
    /// </summary>
    public class WpfDiagram : Diagram
    {
        protected override DiagramNode CreateDiagramNode(IModelEntity modelEntity)
        {
            return new DiagramNodeViewModel(modelEntity, DefaultNodePosition, DefaultNodeSize);
        }
    }
}
