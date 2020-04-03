using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Implementation.Export
{
    internal struct ConnectorSpec
    {
        public long Id { get; set; }
        public long SourceNodeId { get; set; }
        public long TargetNodeId { get; set; }
        public string Stereotype { get; set; }
        public Route Route { get; set; }
    }
}