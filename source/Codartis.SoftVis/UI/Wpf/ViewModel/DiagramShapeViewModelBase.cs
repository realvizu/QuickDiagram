using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for diagram shape view models.
    /// </summary>
    public abstract class DiagramShapeViewModelBase : ModelObserverViewModelBase, IDiagramShapeUi
    {
        public IDiagramShape DiagramShape { get; private set; }
        private Rect _rect;

        protected DiagramShapeViewModelBase(
            IModelService modelService,
            IDiagramService diagramService,
            IDiagramShape diagramShape)
            : base(modelService, diagramService)
        {
            SetDiagramShape(diagramShape);
        }

        protected void SetDiagramShape(IDiagramShape diagramShape)
        {
            DiagramShape = diagramShape;
            Rect = diagramShape.Rect.ToWpf();
        }

        public abstract string StereotypeName { get; }

        public abstract object CloneForImageExport();

        public virtual IEnumerable<IMiniButton> CreateMiniButtons() => Enumerable.Empty<IMiniButton>();

        public override string ToString() => DiagramShape.ToString();

        public Rect Rect
        {
            get { return _rect; }
            protected set
            {
                if (_rect != value)
                {
                    _rect = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}