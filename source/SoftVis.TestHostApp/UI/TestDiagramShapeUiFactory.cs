using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.TestHostApp.UI
{
    public class TestDiagramShapeUiFactory : DiagramShapeUiFactoryBase
    {
        public override DiagramNodeViewModelBase CreateDiagramNodeViewModel(IDiagramService diagramService, IDiagramNode diagramNode)
        {
            return new TypeDiagramNodeViewModel(ModelService, diagramService, (TypeDiagramNode)diagramNode);
        }
    }
}