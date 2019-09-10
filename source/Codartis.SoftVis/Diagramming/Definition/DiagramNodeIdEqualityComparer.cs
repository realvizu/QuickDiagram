using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Compares diagram nodes by ID.
    /// </summary>
    public sealed class DiagramNodeIdEqualityComparer : IEqualityComparer<IDiagramNode>
    {
        public static readonly DiagramNodeIdEqualityComparer Instance = new DiagramNodeIdEqualityComparer();

        public bool Equals(IDiagramNode x, IDiagramNode y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(IDiagramNode obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}