using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.Incremental
{
    internal class LayoutVertexToPointMap : Map<LayoutVertexBase, Point2D>
    { 
        public Rect2D GetRect(LayoutVertexBase vertex)
        {
            var center = Get(vertex);
            return Rect2D.CreateFromCenterAndSize(center, vertex.Size);
        }

        public DiagramNodeLayoutVertexToPointMap GetDiagramNodeLayoutVertexMap()
        {
            var result = new DiagramNodeLayoutVertexToPointMap();

            foreach (var keyValuePair in Dictionary)
            {
                var diagramNodeLayoutVertex = keyValuePair.Key as DiagramNodeLayoutVertex;
                if (diagramNodeLayoutVertex != null)
                    result.Set(diagramNodeLayoutVertex, keyValuePair.Value);
            }

            return result;
        }
    }
}
