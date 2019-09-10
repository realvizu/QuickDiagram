using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Compares diagram connectors by ID.
    /// </summary>
    public sealed class DiagramConnectorIdEqualityComparer : IEqualityComparer<IDiagramConnector>
    {
        public static readonly DiagramConnectorIdEqualityComparer Instance = new DiagramConnectorIdEqualityComparer();

        public bool Equals(IDiagramConnector x, IDiagramConnector y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(IDiagramConnector obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}