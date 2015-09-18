using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.EfficientSugiyama
{
    internal class EdgeToDummyVerticesMap
    {
        private readonly IDictionary<IEdge<IExtent>, IList<SugiVertex>> _edgeToDummyVerticesMap;

        public EdgeToDummyVerticesMap()
        {
            _edgeToDummyVerticesMap = new Dictionary<IEdge<IExtent>, IList<SugiVertex>>();
        }

        public void Add(IEdge<IExtent> edge, IList<SugiVertex> dummyVertices)
        {
            _edgeToDummyVerticesMap[edge] = dummyVertices;
        }

        public IList<SugiVertex> this[IEdge<IExtent> edge]
        {
            get
            {
                IList<SugiVertex> route;
                return _edgeToDummyVerticesMap.TryGetValue(edge, out route) 
                    ? route
                    : null;
            }
        }

        public IEnumerable<DiagramPoint> GetRoutePoints(IEdge<IExtent> edge)
        {
            return this[edge]?.Select(i => i.Center);
        }
    }

}
