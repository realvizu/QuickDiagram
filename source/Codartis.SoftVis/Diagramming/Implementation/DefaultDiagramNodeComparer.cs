using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    public sealed class DefaultDiagramNodeComparer : IComparer<IDiagramNode>
    {
        public int Compare(IDiagramNode x, IDiagramNode y) => x.ModelNode.CompareTo(y.ModelNode);
    }
}