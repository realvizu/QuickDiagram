using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for factories that create view models from diagram shapes.
    /// </summary>
    public abstract class DiagramShapeUiFactoryBase : ModelObserverViewModelBase, IDiagramShapeUiFactory
    {
        protected IDiagramShapeUiRepository DiagramShapeUiRepository { get; private set; }

        protected DiagramShapeUiFactoryBase(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore)
              : base(modelStore, diagramStore)
        {
        }

        public void Initialize(IDiagramShapeUiRepository diagramShapeUiRepository)
        {
            DiagramShapeUiRepository = diagramShapeUiRepository;
        }

        public abstract DiagramNodeViewModelBase CreateDiagramNodeViewModel(IDiagramNode diagramNode);
        public abstract DiagramConnectorViewModel CreateDiagramConnectorViewModel(IDiagramConnector diagramConnector);
    }
}
