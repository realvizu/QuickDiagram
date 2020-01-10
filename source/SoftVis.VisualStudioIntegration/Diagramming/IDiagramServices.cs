using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Defines diagram operations for the application commands package.
    /// </summary>
    public interface IDiagramServices
    {
        IReadOnlyList<IDiagramNode> Nodes { get; }
        IReadOnlyList<IDiagramConnector> Connectors { get; }
        IReadOnlyList<IDiagramShape> Shapes { get; }

        event Action<IDiagramShape> ShapeAdded;
        event Action<IDiagramShape> ShapeRemoved;
        event Action<IDiagramShape> ShapeSelected;
        event Action DiagramCleared;

        IDiagramNode ShowEntity(IModelEntity modelEntity);
        IReadOnlyList<IDiagramNode> ShowEntities(IEnumerable<IModelEntity> modelEntities, CancellationToken cancellationToken, IIncrementalProgress progress);
        IReadOnlyList<IDiagramNode> ShowEntityWithHierarchy(IModelEntity modelEntity, CancellationToken cancellationToken, IIncrementalProgress progress);

        void Clear();
        Task UpdateFromSourceAsync(CancellationToken cancellationToken, IIncrementalProgress progress);
    }
}
