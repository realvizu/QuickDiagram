using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    internal class EdgeToDummyVerticesMap
    {
        private readonly IDictionary<IEdge<ISized>, IList<SugiVertex>> _edgeToDummyVerticesMap;

        public EdgeToDummyVerticesMap()
        {
            _edgeToDummyVerticesMap = new Dictionary<IEdge<ISized>, IList<SugiVertex>>();
        }

        public void Add(IEdge<ISized> edge, IList<SugiVertex> dummyVertices)
        {
            _edgeToDummyVerticesMap[edge] = dummyVertices;
        }

        public IList<SugiVertex> this[IEdge<ISized> edge]
        {
            get
            {
                IList<SugiVertex> route;
                return _edgeToDummyVerticesMap.TryGetValue(edge, out route) 
                    ? route
                    : null;
            }
        }

        public IEnumerable<Point2D> GetRoutePoints(IEdge<ISized> edge)
        {
            return this[edge]?.Select(i => i.Center);
        }
    }

}
