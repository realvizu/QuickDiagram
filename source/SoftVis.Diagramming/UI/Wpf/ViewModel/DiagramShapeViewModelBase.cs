using System;
using System.Collections.Generic;
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

        protected DiagramShapeViewModelBase(IArrangedDiagram diagram, IDiagramShape diagramShape)
            :base(diagram)
        {
            DiagramShape = diagramShape;
        }

        public void Remove() => RemoveRequested?.Invoke(DiagramShape);

        public virtual IEnumerable<MiniButtonViewModelBase> CreateMiniButtonViewModels()
        {
            yield break;
        }

        public override string ToString() => DiagramShape.ToString();
    }
}
