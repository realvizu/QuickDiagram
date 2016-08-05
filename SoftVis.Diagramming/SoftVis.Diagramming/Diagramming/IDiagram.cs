using System;
using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model. 
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// A diagram consists of shapes that represent model elements.
    /// The shapes form a directed graph: some shapes are nodes in the graph and others are connectors between nodes.
    /// A diagram has a layout engine that calculates how to arrange nodes and connectors.
    /// The layout (relative positions and size) also conveys meaning.
    /// </summary>
    public interface IDiagram
    {
        IReadOnlyModel Model { get; }

        IEnumerable<IDiagramNode> Nodes { get; }
        IEnumerable<IDiagramConnector> Connectors { get; }
        IEnumerable<IDiagramShape> Shapes { get; }

        Rect2D ContentRect { get; }

        event EventHandler<IDiagramShape> ShapeAdded;
        event EventHandler<IDiagramShape> ShapeMoved;
        event EventHandler<IDiagramShape> ShapeRemoved;
        event EventHandler<IDiagramShape> ShapeSelected;
        event EventHandler<IDiagramShape> ShapeActivated;
        event EventHandler Cleared;

        IEnumerable<EntityRelationType> GetEntityRelationTypes();

        ConnectorType GetConnectorType(ModelRelationshipType type);
        IEnumerable<IModelEntity> GetUndisplayedRelatedEntities(IDiagramNode diagramNode, EntityRelationType relationType);

        void Clear();
        void ShowItem(IModelItem modelItem);
        void HideItem(IModelItem modelItem);
        void ShowItems(IEnumerable<IModelItem> modelItems);
        void HideItems(IEnumerable<IModelItem> modelItems);

        void SelectShape(IDiagramShape diagramShape);
        void ActivateShape(IDiagramShape diagramShape);
        void RemoveShape(IDiagramShape diagramShape);
    }
}