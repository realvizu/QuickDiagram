using System.Collections.Generic;
using System.Linq;
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
        public IDiagramShape DiagramShape { get; private set; }
        public IPayloadUi PayloadUi { get; private set; }
        private Rect _rect;

        protected DiagramShapeViewModelBase(
            IModelEventSource modelEventSource,
            IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramShape diagramShape,
            [CanBeNull] IPayloadUi payloadUi)
            : base(modelEventSource, diagramEventSource)
        {
            UpdateDiagramShape(diagramShape, payloadUi);
        }

        protected void UpdateDiagramShape([NotNull] IDiagramShape diagramShape, [CanBeNull] IPayloadUi payloadUi)
        {
            DiagramShape = diagramShape;
            PayloadUi = payloadUi;
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