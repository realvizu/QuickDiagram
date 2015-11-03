using System;
using Codartis.SoftVis.Diagramming.Layout.Incremental;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    internal class TestPositioningVertex : PositioningVertexBase
    {
        public override string Name { get; }

        public TestPositioningVertex(PositioningGraph graph, string name, bool isFloating) 
            : base(graph, isFloating)
        {
            Name = name;
        }

        public override int Priority => 1;
        public override double Width => 20;
        public override double Height => 10;

        protected bool Equals(TestPositioningVertex other)
        {
            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestPositioningVertex) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
        }

        public static bool operator ==(TestPositioningVertex left, TestPositioningVertex right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestPositioningVertex left, TestPositioningVertex right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
