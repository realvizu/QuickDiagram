using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// The graph formed by the nodes and connectors of a diagram.
    /// </summary>
    public class DiagramGraph : BidirectionalGraph<DiagramNode, DiagramConnector>
    {
        public DiagramNode FindNode(UmlModelElement umlModelElement)
        {
            return Vertices.FirstOrDefault(i => i.ModelElement == umlModelElement);
        }

        internal void PositionNodes(IDictionary<DiagramNode, DiagramPoint> nodePositions)
        {
            if (nodePositions != null && nodePositions.Any())
            {
                foreach (var vertex in Vertices)
                    vertex.Position = nodePositions[vertex];
            }
        }
    }
}
