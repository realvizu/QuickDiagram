using Codartis.SoftVis.Diagramming.Shapes;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels
{
    /// <summary>
    /// A diagram connector that support data binding and can be used as a ViewModel from WPF controls.
    /// </summary>
    internal class DiagramConnectorViewModel : DiagramConnector
    {
        public DiagramConnectorViewModel(IModelRelationship relationship, DiagramNodeViewModel source, DiagramNodeViewModel target) 
            : base(relationship, source, target)
        {
        }
    }
}
