using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates positions whenever vertices and edges are added.
    /// Sends events about vertex position and edge route changes.
    /// </summary>
    /// <remarks>
    /// Layout rules:
    /// <para>Adding a new node adds it to the first layer in node name order.</para>
    /// <para>Adding a new inheritance connection moves the source node under the target node.
    /// The source node brings all its children with it.</para>
    /// <para>If the source node has siblings then it is placed among them based on node name order.</para>
    /// <para>If the source node has no siblings then it is placed between the children of the parent's preceding and following nodes.
    /// It ensures that there won't be any inheritance edge crossings.</para>
    /// <para>The ordering of the nodes' horizontal position always correspond to their order in the layer. 
    /// To maintain this correspondance some nodes can be pushed to the left or right when placing a new node.</para>
    /// <para>Pushing away nodes also ensures that there are no overlapping nodes.</para>
    /// <para>If a node is pushed then all its descendants are pushed with it.</para>
    /// <para>When a tree is moved it is first set to "floating", that is it won't cause any vertex overlaps.
    /// (To avoid collision with itself.)</para>
    /// <para>When a vertex is placed (its center property is set) then it stops floating.</para>
    /// <para>When a vertex is placed (for any reason) its vertical position is acquired from its layer.</para>
    /// <para>When a vertex is pushed then its parent is centered again to its child block (recursive on parents upwards),
    /// possibly pushing away other nodes.</para>
    /// Events:
    /// <para>The layout engine subscribes to the vertices center property changed event.</para>
    /// <para>DiagramNodeCenterChanged event is fired whenever a vertex is moved.</para>
    /// <para>DiagramConnectorRouteChanged event is fired when the edge is first added and whenever any of the end-vertices are moved.</para>
    /// </remarks>
    internal sealed class IncrementalLayoutEngine
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly LayoutGraph _layoutGraph;
        private readonly Dictionary<DiagramNode, LayoutVertexBase> _diagramNodeToLayoutVertexMap;
        private readonly Dictionary<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly Dictionary<DummyLayoutVertex, DiagramConnector> _dummyVertexToDiagramConnectorMap;

        public int TotalVertexMoveCount { get; private set; }
        public ILayoutActionGraph LastLayoutActionGraph { get; private set; }

        public event EventHandler<RectMove> DiagramNodeCenterChanged;
        public event EventHandler<Route> DiagramConnectorRouteChanged;

        public IncrementalLayoutEngine(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;
            _layoutGraph = new LayoutGraph(verticalGap);
            _diagramNodeToLayoutVertexMap = new Dictionary<DiagramNode, LayoutVertexBase>();
            _diagramConnectorToLayoutPathMap = new Dictionary<DiagramConnector, LayoutPath>();
            _dummyVertexToDiagramConnectorMap = new Dictionary<DummyLayoutVertex, DiagramConnector>();
        }

        public void Clear()
        {
            _layoutGraph.Clear();
            _diagramNodeToLayoutVertexMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
            _dummyVertexToDiagramConnectorMap.Clear();
            TotalVertexMoveCount = 0;
        }

        public void Add(DiagramNode diagramNode)
        {
            RecordVertexLayoutActions(vertexLayoutLogic =>
            {
                var newLayoutVertex = CreateLayoutVertex(diagramNode);

                _diagramNodeToLayoutVertexMap.Add(diagramNode, newLayoutVertex);

                vertexLayoutLogic.AddVertex(newLayoutVertex);
            });
        }

        public void Remove(DiagramNode diagramNode)
        {
            RecordVertexLayoutActions(vertexLayoutLogic =>
            {
                var layoutVertex = GetLayoutVertex(diagramNode);

                vertexLayoutLogic.RemoveVertex(layoutVertex);

                _diagramNodeToLayoutVertexMap.Remove(diagramNode);
            });
        }

        public void Add(DiagramConnector diagramConnector)
        {
            RecordEdgeLayoutActions(edgeLayoutLogic =>
            {
                var newLayoutPath = CreateLayoutPath(diagramConnector);

                UpdateDiagramConnectorToLayoutPathMap(diagramConnector, newLayoutPath);

                edgeLayoutLogic.AddPath(newLayoutPath);
            });
        }

        public void Remove(DiagramConnector diagramConnector)
        {
            RecordEdgeLayoutActions(edgeLayoutLogic =>
            {
                var layoutPath = GetLayoutPath(diagramConnector);
                edgeLayoutLogic.RemovePath(layoutPath);

                foreach (var dummyVertex in layoutPath.GetInterimVertices())
                    _dummyVertexToDiagramConnectorMap.Remove(dummyVertex);

                _diagramConnectorToLayoutPathMap.Remove(diagramConnector);
            });
        }

        private void RecordVertexLayoutActions(Action<LayoutActionGraph, VertexLayoutLogic> action)
        {
            var layoutActionGraph = new LayoutActionGraph();
            var vertexLayoutLogic = CreateVertexLayoutLogic(layoutActionGraph);

            action(layoutActionGraph, vertexLayoutLogic);

            LastLayoutActionGraph = layoutActionGraph;
            TotalVertexMoveCount += layoutActionGraph.VertexMoveCount;
        }

        private void RecordVertexLayoutActions(Action<VertexLayoutLogic> action)
        {
            RecordVertexLayoutActions((layoutActionGraph, vertexLayoutLogic) =>
            {
                action(vertexLayoutLogic);
            });
        }

        private void RecordEdgeLayoutActions(Action<EdgeLayoutLogic> action)
        {
            RecordVertexLayoutActions((layoutActionGraph, vertexLayoutLogic) =>
            {
                var edgeLayoutLogic = CreateEdgeLayoutLogic(layoutActionGraph, vertexLayoutLogic);

                action(edgeLayoutLogic);
            });
        }

        private VertexLayoutLogic CreateVertexLayoutLogic(LayoutActionGraph layoutActionGraph)
        {
            return new VertexLayoutLogic(_horizontalGap, _verticalGap, _layoutGraph, layoutActionGraph);
        }

        private EdgeLayoutLogic CreateEdgeLayoutLogic(LayoutActionGraph layoutActionGraph, VertexLayoutLogic vertexLayoutLogic)
        {
            var edgeLayoutLogic = new EdgeLayoutLogic(_horizontalGap, _verticalGap, _layoutGraph, layoutActionGraph, vertexLayoutLogic);
            edgeLayoutLogic.LayoutPathChanged += OnLayoutPathChanged;
            return edgeLayoutLogic;
        }

        private LayoutVertexBase CreateLayoutVertex(DiagramNode diagramNode)
        {
            var newLayoutVertex = new DiagramNodeLayoutVertex(_layoutGraph, diagramNode, isFloating: true);
            newLayoutVertex.CenterChanged += OnDiagramNodeLayoutVertexCenterChanged;
            return newLayoutVertex;
        }

        private LayoutVertexBase GetLayoutVertex(DiagramNode diagramNode)
        {
            return _diagramNodeToLayoutVertexMap[diagramNode];
        }

        //private void RemoveVertex(LayoutVertexBase layoutVertex)
        //{
        //    foreach (var inEdge in layoutVertex.InEdges.ToList())
        //    {
        //        var sourceVertex = inEdge.Source;
        //        if (sourceVertex.IsDummy)
        //        {
        //            var path = _diagramConnectorToLayoutPathMap[inEdge.DiagramConnector];
        //            var modifiedPath = _edgeLayoutLogic.RemoveVertexFromPath(sourceVertex, path);
        //            _diagramConnectorToLayoutPathMap[inEdge.DiagramConnector] = modifiedPath;
        //            RemoveVertex(sourceVertex);
        //        }
        //    }
        //}

        private LayoutEdge CreateLayoutEdge(DiagramConnector diagramConnector)
        {
            // TODO: turn edge direction if needed!
            var isReversed = false;

            var sourceLayoutVertex = _diagramNodeToLayoutVertexMap[diagramConnector.Source];
            var targetLayoutVertex = _diagramNodeToLayoutVertexMap[diagramConnector.Target];

            var newLayoutEdge = new LayoutEdge(_layoutGraph, sourceLayoutVertex, targetLayoutVertex,
                diagramConnector, isReversed);

            return newLayoutEdge;
        }

        private LayoutPath CreateLayoutPath(DiagramConnector diagramConnector)
        {
            var layoutEdge = CreateLayoutEdge(diagramConnector);

            var childLayerIndex = layoutEdge.Source.GetLayerIndex();
            var parentLayerIndex = layoutEdge.Target.GetLayerIndex();
            var intermediateLayers = Math.Max(childLayerIndex - parentLayerIndex - 1, 0);

            var dummyVertices = CreateDummyVertices(intermediateLayers).ToArray();
            foreach (var dummyVertex in dummyVertices)
                _dummyVertexToDiagramConnectorMap.Add(dummyVertex, diagramConnector);

            return new LayoutPath(layoutEdge, dummyVertices);
        }

        private LayoutPath GetLayoutPath(DiagramConnector diagramConnector)
        {
            return _diagramConnectorToLayoutPathMap[diagramConnector];
        }

        private IEnumerable<DummyLayoutVertex> CreateDummyVertices(int count)
        {
            for (var i = 0; i < count; i++)
                yield return CreateDummyVertex();
        }

        private DummyLayoutVertex CreateDummyVertex()
        {
            var dummyLayoutVertex = new DummyLayoutVertex(_layoutGraph, isFloating: true);
            dummyLayoutVertex.CenterChanged += OnDummyLayoutVertexCenterChanged;
            return dummyLayoutVertex;
        }

        private void UpdateDiagramConnectorToLayoutPathMap(DiagramConnector diagramConnector, LayoutPath layoutPath)
        {
            if (_diagramConnectorToLayoutPathMap.ContainsKey(diagramConnector))
                _diagramConnectorToLayoutPathMap[diagramConnector] = layoutPath;
            else
                _diagramConnectorToLayoutPathMap.Add(diagramConnector, layoutPath);
        }

        private void OnDiagramNodeLayoutVertexCenterChanged(object sender, RectMove args)
        {
            var diagramNodeLayoutVertex = sender as DiagramNodeLayoutVertex;
            if (diagramNodeLayoutVertex != null)
            {
                FireDiagramNodeCenterChanged(diagramNodeLayoutVertex, args);
            }
        }

        private void OnDummyLayoutVertexCenterChanged(object sender, RectMove args)
        {
            var dummyLayoutVertex = sender as DummyLayoutVertex;
            if (dummyLayoutVertex != null)
            {
                var diagramConnector = _dummyVertexToDiagramConnectorMap[dummyLayoutVertex];
                FireDiagramConnectorRouteChanged(diagramConnector);
            }
        }

        private void OnLayoutPathChanged(object sender, LayoutPath layoutPath)
        {
            var diagramConnector = layoutPath.First().DiagramConnector;

            UpdateDiagramConnectorToLayoutPathMap(diagramConnector, layoutPath);

            FireDiagramConnectorRouteChanged(diagramConnector);
        }

        private void FireDiagramNodeCenterChanged(DiagramNodeLayoutVertex diagramNodeLayoutVertex, RectMove args)
        {
            DiagramNodeCenterChanged?.Invoke(diagramNodeLayoutVertex.DiagramNode, args);

            foreach (var layoutEdge in _layoutGraph.GetAllEdges(diagramNodeLayoutVertex))
                FireDiagramConnectorRouteChanged(layoutEdge.DiagramConnector);
        }

        private void FireDiagramConnectorRouteChanged(DiagramConnector diagramConnector)
        {
            var route = GetLayoutPath(diagramConnector).GetRoute();
            DiagramConnectorRouteChanged?.Invoke(diagramConnector, route);
        }
    }
}
