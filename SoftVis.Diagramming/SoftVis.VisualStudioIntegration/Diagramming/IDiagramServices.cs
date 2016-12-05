using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

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

        void ShowEntity(IRoslynBasedModelEntity modelEntity);
        void ShowEntityWithHierarchy(IRoslynBasedModelEntity modelEntity, CancellationToken cancellationToken, IIncrementalProgress progress);
        void ShowItemsWithProgress(IEnumerable<IModelItem> modelItems, CancellationToken cancellationToken, IIncrementalProgress progress);

        void Clear();
        void UpdateFromCode(CancellationToken cancellationToken, IIncrementalProgress progress);
    }
}
