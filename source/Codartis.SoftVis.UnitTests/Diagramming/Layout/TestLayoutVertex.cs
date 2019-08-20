using System;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UnitTests.Diagramming.Layout
{
    internal class TestLayoutVertex : DiagramNodeLayoutVertex
    {
        public TestLayoutVertex(string name, int priority = 1) 
            : base(new TestDiagramNode(),  name, priority)
        {
        }

        public override Size2D Size => new Size2D(20, 10);

        public override bool IsDummy => false;

        protected bool Equals(TestLayoutVertex other)
        {
            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestLayoutVertex) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
        }

        public static bool operator ==(TestLayoutVertex left, TestLayoutVertex right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestLayoutVertex left, TestLayoutVertex right)
        {
            return !Equals(left, right);
        }
    }
}
