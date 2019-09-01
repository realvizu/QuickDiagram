﻿using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A part of a diagram that is laid out together.
    /// Consists of diagram nodes and connectors that form a directed graph.
    /// </summary>
    public interface ILayoutGroup
    {
        /// <summary>
        /// Returns all nodes in the layout group.
        /// </summary>
        [NotNull]
        IImmutableSet<IDiagramNode> Nodes { get; }

        /// <summary>
        /// Returns all connectors in the layout group.
        /// </summary>
        [NotNull]
        IImmutableSet<IDiagramConnector> Connectors { get; }

        /// <summary>
        /// Returns all nodes in the layout group, including embedded layout groups.
        /// </summary>
        [NotNull]
        IImmutableSet<IDiagramNode> NodesRecursive { get; }

        /// <summary>
        /// Returns all connectors in the layout group, including embedded layout groups.
        /// </summary>
        [NotNull]
        IImmutableSet<IDiagramConnector> ConnectorsRecursive { get; }

        /// <summary>
        /// The position and size of the layout group relative to its parent (if any).
        /// </summary>
        Rect2D Rect { get; }

        [NotNull]
        ILayoutGroup AddNode([NotNull] IDiagramNode node, ModelNodeId? parentNodeId);

        [NotNull]
        ILayoutGroup UpdateNode([NotNull] IDiagramNode updatedNode);

        [NotNull]
        ILayoutGroup RemoveNode(ModelNodeId nodeId);

        [NotNull]
        ILayoutGroup AddConnector([NotNull] IDiagramConnector connector);

        [NotNull]
        ILayoutGroup UpdateConnector([NotNull] IDiagramConnector updatedConnector);

        [NotNull]
        ILayoutGroup RemoveConnector(ModelRelationshipId connectorId);

        [NotNull]
        ILayoutGroup Clear();
    }
}