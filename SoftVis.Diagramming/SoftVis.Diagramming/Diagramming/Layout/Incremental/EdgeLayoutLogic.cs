using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Implements the edge layout logic of the incremental layout engine.
    /// </summary>
    internal sealed class EdgeLayoutLogic : LayoutLogicBase
    {
        private readonly VertexLayoutLogic _vertexLayoutLogic;

        public event EventHandler<LayoutPath> LayoutPathChanged;

        public EdgeLayoutLogic(double horizontalGap, double verticalGap, LayoutGraph layoutGraph,
            LayoutActionGraph layoutActionGraph, VertexLayoutLogic vertexLayoutLogic)
             : base(horizontalGap, verticalGap, layoutGraph, layoutActionGraph)
        {
            _vertexLayoutLogic = vertexLayoutLogic;
        }

        public void AddPath(LayoutPath layoutPath)
        {
            var layoutAction = RecordPathAction("AddPath", layoutPath);

            LayoutGraph.AddPath(layoutPath);

            foreach (var edgeInPath in layoutPath.Reverse())
                _vertexLayoutLogic.PositionVertex(edgeInPath.Source, edgeInPath.Target, layoutAction);

            // minden módosult path-ra
            LayoutPathChanged?.Invoke(this, layoutPath);
        }

        public void RemovePath(LayoutPath layoutPath)
        {
            var layoutAction = RecordPathAction("RemovePath", layoutPath);

            var dummyVerticesInPath = layoutPath.InterimVertices;

            // It would be too early to remove the edges now
            // because we'll need them at vertex removal to calculate siblings. 

            // Removing the dummy vertices will remove the attached edges too.
            foreach (var dummyVertex in dummyVerticesInPath)
                _vertexLayoutLogic.RemoveVertex(dummyVertex, layoutAction);

            if (layoutPath.Length == 1)
                LayoutGraph.RemoveEdge(layoutPath[0]);
        }

        public LayoutPath RemoveInterimVertexFromPath(LayoutPath layoutPath)
        {
            if (!layoutPath.InterimVertices.Any())
                throw new InvalidOperationException("Path has no interim vertex to remove.");

            var edgesInPath = layoutPath.ToList();

            var previousEdge = edgesInPath[0];
            var currentEdge = edgesInPath[1];

            var mergedEdge = new LayoutEdge(LayoutGraph, previousEdge.Source, currentEdge.Target,
                currentEdge.DiagramConnector, currentEdge.IsReversed);

            LayoutGraph.AddEdge(mergedEdge);
            edgesInPath.Insert(0, mergedEdge);

            LayoutGraph.RemoveEdge(previousEdge);
            edgesInPath.Remove(previousEdge);

            LayoutGraph.RemoveEdge(currentEdge);
            edgesInPath.Remove(currentEdge);

            return new LayoutPath(edgesInPath);
        }
    }
}