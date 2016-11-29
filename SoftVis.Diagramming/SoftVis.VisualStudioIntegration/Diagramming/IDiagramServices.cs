using System;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Defines diagram operations for the application commands package.
    /// </summary>
    public interface IDiagramServices
    {
        event Action<IDiagramShape> ShapeAdded;
        event Action<IDiagramShape> ShapeRemoved;
        event Action<IDiagramShape> ShapeSelected;
        event Action<IDiagramShape> ShapeActivated;
        event Action Cleared;

        void ShowModelEntity(IRoslynBasedModelEntity modelEntity);
        void ShowModelEntityWithHierarchy(IRoslynBasedModelEntity modelEntity, CancellationToken cancellationToken, IProgress<int> progress);

        void Clear();
    }
}
