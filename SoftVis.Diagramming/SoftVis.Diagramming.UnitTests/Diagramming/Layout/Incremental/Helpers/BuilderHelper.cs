using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal static class BuilderHelper
    {
        private const string LeftToRightSeparator = "->";
        private const string RightToLeftSeparator = "<-";

        public static string[] PathSpecificationToVertexNames(string pathSpecification)
        {
            var leftToRight = pathSpecification.Contains(LeftToRightSeparator);
            var rightToLeft = pathSpecification.Contains(RightToLeftSeparator);

            if (leftToRight && rightToLeft)
                throw new ArgumentException("Do not mix left-to-right and right-to-left arrows in one path specification!");

            var separator = leftToRight ? LeftToRightSeparator : RightToLeftSeparator;
            var vertices = pathSpecification.Split(new[] { separator }, StringSplitOptions.None);

            return leftToRight
                ? vertices.ToArray()
                : vertices.Reverse().ToArray();
        }

        public static IEnumerable<EdgeSpecification> StringToEdgeSpecifications(string pathSpecification)
        {
            var vertexNames = PathSpecificationToVertexNames(pathSpecification);

            for (var i = 0; i < vertexNames.Length - 1; i++)
                yield return new EdgeSpecification(vertexNames[i], vertexNames[i + 1]);
        }

        public static EdgeSpecification StringToEdgeSpecification(string edgeSpecification)
        {
            return StringToEdgeSpecifications(edgeSpecification).FirstOrDefault();
        }
    }
}