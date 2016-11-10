using System;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for diagram shape view models.
    /// </summary>
    public abstract class DiagramShapeViewModelBase : DiagramViewModelBase
    {
        public IDiagramShape DiagramShape { get; }

        public event Action<IDiagramShape> RemoveRequested;
        public event Action<DiagramShapeViewModelBase> FocusRequested;

        protected DiagramShapeViewModelBase(IArrangedDiagram diagram, IDiagramShape diagramShape)
            :base(diagram)
        {
            DiagramShape = diagramShape;
        }

        public void Remove() => RemoveRequested?.Invoke(DiagramShape);
        public void Focus() => FocusRequested?.Invoke(this);

        public override string ToString() => DiagramShape.ToString();
    }
}
