using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Modeling2;
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
        // Remove when DiagramBuilder is separated.
        IModelBuilder ModelBuilder { get; }

        IReadOnlyList<IDiagramNode> Nodes { get; }
        IReadOnlyList<IDiagramConnector> Connectors { get; }
        IReadOnlyList<IDiagramShape> Shapes { get; }

        event Action<IDiagramShape> ShapeAdded;
        event Action<IDiagramShape> ShapeRemoved;
        event Action<IDiagramShape> ShapeSelected;
        event Action DiagramCleared;

        //IEnumerable<EntityRelationType> GetEntityRelationTypes();
        //ConnectorType GetConnectorType(ModelRelationshipType type);

        IDiagramShape ShowModelItem(IModelItem modelItem);

        IReadOnlyList<IDiagramShape> ShowModelItems(IEnumerable<IModelItem> modelItems,
            CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null);

        //void HideModelItem(IModelItem modelItem);

        //void HideModelItems(IEnumerable<IModelItem> modelItems,
        //    CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null);

        void Clear();

        void SelectDiagramShape(IDiagramShape diagramShape);
        void RemoveDiagramShape(IDiagramShape diagramShape);

        //IReadOnlyList<IModelEntity> GetUndisplayedRelatedModelEntities(IDiagramNode diagramNode, EntityRelationType relationType);
    }
}