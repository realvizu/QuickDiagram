using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    internal class LayoutVertex
    {
        private IExtent OriginalVertex { get; }

        public LayoutVertex(IExtent originalVertex)
        {
            OriginalVertex = originalVertex;
        }
    }
}
