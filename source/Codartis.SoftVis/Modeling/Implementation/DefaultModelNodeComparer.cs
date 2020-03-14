using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Modeling.Implementation
{
    public sealed class DefaultModelNodeComparer : IComparer<IModelNode>
    {
        public int Compare(IModelNode x, IModelNode y) => x.CompareTo(y);
    }
}