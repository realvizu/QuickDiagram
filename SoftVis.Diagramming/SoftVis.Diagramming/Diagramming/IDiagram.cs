using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling;

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
        event Action<IDiagramShape> ShapeActivated;
        event Action Cleared;

        IEnumerable<EntityRelationType> GetEntityRelationTypes();
        ConnectorType GetConnectorType(ModelRelationshipType type);

        void ShowItem(IModelItem modelItem);
        void HideItem(IModelItem modelItem);
        void ShowItems(IEnumerable<IModelItem> modelItems);
        void HideItems(IEnumerable<IModelItem> modelItems);
        void Clear();

        void SelectShape(IDiagramShape diagramShape);
        void ActivateShape(IDiagramShape diagramShape);
        void RemoveShape(IDiagramShape diagramShape);

        IEnumerable<IModelEntity> GetUndisplayedRelatedEntities(IDiagramNode diagramNode, EntityRelationType relationType);
    }
}