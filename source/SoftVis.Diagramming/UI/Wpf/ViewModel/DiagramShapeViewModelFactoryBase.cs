using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for factories that create view models from diagram shapes.
    /// </summary>
    public abstract class DiagramShapeViewModelFactoryBase : DiagramViewModelBase
    {
        protected IDiagramShapeViewModelRepository DiagramShapeViewModelRepository { get; private set; }

        protected DiagramShapeViewModelFactoryBase(IArrangedDiagram diagram)
              : base(diagram)
        {
        }

        public void Initialize(IDiagramShapeViewModelRepository diagramShapeViewModelRepository)
        {
            DiagramShapeViewModelRepository = diagramShapeViewModelRepository;
        }

        public abstract DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape);
    }
}
