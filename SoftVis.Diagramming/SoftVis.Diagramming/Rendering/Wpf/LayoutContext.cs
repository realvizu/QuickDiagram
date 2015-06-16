using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf
{
    public class LayoutContext
    {
        public IDictionary<DiagramNode, Point> Positions { get; private set; }

        public IDictionary<DiagramNode, Size> Sizes { get; private set; }

        public DiagramGraph Graph { get; private set; }

        public LayoutContext(DiagramGraph graph, IDictionary<DiagramNode, Point> positions, IDictionary<DiagramNode, Size> sizes)
        {
            Graph = graph;
            Positions = positions;
            Sizes = sizes;
        }
    }
}
