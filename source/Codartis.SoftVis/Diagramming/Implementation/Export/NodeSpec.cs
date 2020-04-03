using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Implementation.Export
{
    internal struct NodeSpec
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Stereotype { get; set; }
        public long? ParentNodeId { get; set; }
        public DateTime AddedAt { get; set; }
        public Size2D Size { get; set; }
        public Size2D ChildrenAreaSize { get; set; }
        public Point2D Center { get; set; }
    }
}