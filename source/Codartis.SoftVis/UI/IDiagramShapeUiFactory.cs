using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates diagram shape UI instances.
    /// </summary>
    public interface IDiagramShapeUiFactory
    {
        void Initialize(IModelService modelService, IDiagramShapeUiRepository diagramShapeUiRepository);

        IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramService diagramService,
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker);

        IDiagramConnectorUi CreateDiagramConnectorUi(IDiagramService diagramService, IDiagramConnector diagramConnector);
    }
}