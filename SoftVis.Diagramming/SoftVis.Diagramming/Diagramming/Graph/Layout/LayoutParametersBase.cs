using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.Graph.Layout.EdgeRouting;

namespace Codartis.SoftVis.Diagramming.Graph.Layout
{
    public class LayoutParametersBase : ILayoutParameters
    {
        public EdgeRoutingType EdgeRoutingType { get; set; }
    }
}
