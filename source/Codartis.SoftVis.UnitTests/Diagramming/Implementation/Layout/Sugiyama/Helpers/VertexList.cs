using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Helpers
{
    internal abstract class VertexList : IEnumerable<string>
    {
        private const string LeftToRightSeparator = "->";
        private const string RightToLeftSeparator = "<-";

        public string[] VertexNames { get; }

        protected VertexList(IEnumerable<string> vertexNames)
        {
            VertexNames = vertexNames as string[] ?? vertexNames.ToArray();
        }

        protected static IEnumerable<string> InternalParse(string pathString)
        {
            var leftToRight = pathString.Contains(LeftToRightSeparator);
            var rightToLeft = pathString.Contains(RightToLeftSeparator);

            if (leftToRight && rightToLeft)
                throw new ArgumentException("Do not mix left-to-right and right-to-left arrows in one path specification!");

            var separator = leftToRight ? LeftToRightSeparator : RightToLeftSeparator;
            var vertexNames = pathString.Split(new[] { separator }, StringSplitOptions.None);

            return leftToRight
                ? vertexNames
                : vertexNames.Reverse();
        }

        public IEnumerator<string> GetEnumerator() => VertexNames.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}