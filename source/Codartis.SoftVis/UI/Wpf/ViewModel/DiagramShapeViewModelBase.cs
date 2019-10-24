using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for diagram shape view models.
    /// </summary>
    public abstract class DiagramShapeViewModelBase : ModelObserverViewModelBase, IDiagramShapeUi
    {
        public IDiagramShape DiagramShape { get; protected set; }

        protected DiagramShapeViewModelBase(
            IModelService modelService,
            IDiagramService diagramService,
            IDiagramShape diagramShape)
            : base(modelService, diagramService)
        {
            DiagramShape = diagramShape;
        }

        public abstract string Stereotype { get; }

        public abstract object CloneForImageExport();

        public virtual IEnumerable<IMiniButton> CreateMiniButtons()
        {
            yield break;
        }

        public override string ToString() => DiagramShape.ToString();
    }
}