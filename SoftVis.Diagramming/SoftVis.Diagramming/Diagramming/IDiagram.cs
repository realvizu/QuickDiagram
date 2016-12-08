using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model. 
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// A diagram consists of shapes that represent model elements.
    /// The shapes form a directed graph: some shapes are nodes in the graph and others are connectors between nodes.
    /// </summary>
    public interface IDiagram
    {
        IReadOnlyModel Model { get; }

        IEnumerable<IDiagramNode> Nodes { get; }
        IEnumerable<IDiagramConnector> Connectors { get; }
        IEnumerable<IDiagramShape> Shapes { get; }

        event Action<IDiagramShape> ShapeAdded;
        event Action<IDiagramShape> ShapeRemoved;
        event Action<IDiagramShape> ShapeSelected;
        event Action<IDiagramShape> ShowSourceRequested;
        event Action DiagramCleared;

        IEnumerable<EntityRelationType> GetEntityRelationTypes();
        ConnectorType GetConnectorType(ModelRelationshipType type);

        IDiagramShape ShowItem(IModelItem modelItem);

        List<IDiagramShape> ShowItems(IEnumerable<IModelItem> modelItems,
            CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null);

        void HideItem(IModelItem modelItem);

        void HideItems(IEnumerable<IModelItem> modelItems,
            CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null);

        void Clear();

        void SelectShape(IDiagramShape diagramShape);
        void ShowSource(IDiagramShape diagramShape);
        void RemoveShape(IDiagramShape diagramShape);

        IEnumerable<IModelEntity> GetUndisplayedRelatedEntities(IDiagramNode diagramNode, EntityRelationType relationType);
    }
}