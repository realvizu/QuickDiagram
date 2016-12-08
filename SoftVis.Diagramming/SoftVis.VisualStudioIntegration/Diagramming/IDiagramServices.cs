using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Defines diagram operations for the application commands package.
    /// </summary>
    public interface IDiagramServices
    {
        IEnumerable<DiagramNode> Nodes { get; }
        IEnumerable<DiagramConnector> Connectors { get; }
        IEnumerable<DiagramShape> Shapes { get; }

        event Action<IDiagramShape> ShapeAdded;
        event Action<IDiagramShape> ShapeRemoved;
        event Action<IDiagramShape> ShapeSelected;
        event Action DiagramCleared;

        event Action<IDiagramShape> ShowSourceRequested;
        event Action<List<IModelItem>> ShowItemsRequested;

        IDiagramNode ShowEntity(IModelEntity modelEntity);
        List<IDiagramNode> ShowEntities(IEnumerable<IModelEntity> modelEntities, CancellationToken cancellationToken, IIncrementalProgress progress);
        List<IDiagramNode> ShowEntityWithHierarchy(IModelEntity modelEntity, CancellationToken cancellationToken, IIncrementalProgress progress);

        void Clear();
        void UpdateFromSource(CancellationToken cancellationToken, IIncrementalProgress progress);
    }
}
