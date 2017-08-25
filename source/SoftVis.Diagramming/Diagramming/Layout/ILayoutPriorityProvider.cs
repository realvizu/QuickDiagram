using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Provides layout priority information about diagram nodes.
    /// </summary>
    public interface ILayoutPriorityProvider
    {
        int GetPriority(IDiagramNode diagramNode);
    }
}
