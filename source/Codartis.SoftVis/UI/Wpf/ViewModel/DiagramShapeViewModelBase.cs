using System.Windows;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for diagram shape view models.
    /// </summary>
    public abstract class DiagramShapeViewModelBase : ModelObserverViewModelBase, IDiagramShapeUi
    {
        [NotNull] public IDiagramShape DiagramShape { get; private set; }
        private Rect _rect;

        protected DiagramShapeViewModelBase(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramShape diagramShape)
            : base(modelEventSource, diagramEventSource)
        {
            SetDiagramShapeProperties(diagramShape);
        }

        protected void SetDiagramShapeProperties([NotNull] IDiagramShape diagramShape)
        {
            DiagramShape = diagramShape;
            Rect = diagramShape.AbsoluteRect.ToWpf();
        }

        public abstract string StereotypeName { get; }

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

        [NotNull]
        public abstract IDiagramShapeUi CloneForImageExport();

        public override string ToString() => DiagramShape.ToString();
    }
}