using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramFixtures
{
    public static class DiagramNodeControlFactory
    {
        public static DiagramNodeControl CreateFrom(DiagramNode diagramNode)
        {
            return new DiagramNodeControl
            {
                DiagramNode = diagramNode,
                NodeType = diagramNode.GetType().ToString(),
                DataContext = diagramNode,
            };
        }
    }
}
