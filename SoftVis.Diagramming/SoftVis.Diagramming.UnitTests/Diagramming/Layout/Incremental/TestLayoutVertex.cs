using System;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    internal class TestLayoutVertex : LayoutVertexBase
    {
        public override string Name { get; }
        public override int Priority { get; }

        public TestLayoutVertex(string name, bool isFloating, int priority = 1) 
            : base(isFloating)
        {
            Name = name;
            Priority = priority;
        }

        public override bool IsDummy => false;
        public override Size2D Size => new Size2D(20,10);
        
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

        public override string ToString()
        {
            return Name;
        }
    }
}
