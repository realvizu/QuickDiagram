using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    internal static class BuilderHelper
    {
        public static IEnumerable<string> GetVertexNames(string pathSpecification)
        {
            return pathSpecification.Split(new[] { "<-" }, StringSplitOptions.None).ToArray();
        }

        public static IEnumerable<EdgeSpecification> GetEdgeSpecifications(string pathSpecification)
        {
            var vertexNames = pathSpecification.Split(new[] { "<-" }, StringSplitOptions.None).ToArray();

            for (var i = 0; i < vertexNames.Length - 1; i++)
                yield return new EdgeSpecification(vertexNames[i + 1], vertexNames[i]);
        }
    }
}