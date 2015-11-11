using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal abstract class BuilderBase
    {
        private const string LeftToRightSeparator = "->";
        private const string RightToLeftSeparator = "<-";

        protected IEnumerable<EdgeSpecification> StringToEdgeSpecifications(string pathSpecification)
        {
            var vertexNames = PathSpecificationToVertexNames(pathSpecification).ToArray();

            for (var i = 0; i < vertexNames.Length - 1; i++)
                yield return new EdgeSpecification(vertexNames[i], vertexNames[i + 1]);
        }

        protected virtual IEnumerable<string> PathSpecificationToVertexNames(string pathSpecification)
        {
            var leftToRight = pathSpecification.Contains(LeftToRightSeparator);
            var rightToLeft = pathSpecification.Contains(RightToLeftSeparator);

            if (leftToRight && rightToLeft)
                throw new ArgumentException("Do not mix left-to-right and right-to-left arrows in one path specification!");

            var separator = leftToRight ? LeftToRightSeparator : RightToLeftSeparator;
            var vertices = pathSpecification.Split(new[] { separator }, StringSplitOptions.None);

            return leftToRight
                ? vertices
                : vertices.Reverse();
        }

        protected LayoutEdge CreateLayoutEdge(EdgeSpecification edgeSpecification)
        {
            return CreateLayoutEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
        }

        protected LayoutEdge CreateLayoutEdge(string sourceVertexName, string targetVertexName)
        {
            var source = CreateLayoutVertex(sourceVertexName);
            var target = CreateLayoutVertex(targetVertexName);
            return CreateLayoutEdge(source, target);
        }

        protected LayoutEdge CreateLayoutEdge(LayoutVertexBase source, LayoutVertexBase target)
        {
            return new LayoutEdge(source, target, null);
        }

        protected static LayoutVertexBase CreateLayoutVertex(string name, int priority = 1)
        {
            return name.StartsWith("*")
                ? (LayoutVertexBase)CreateDummyLayoutVertex(name)
                : CreateTestLayoutVertex(name, priority);
        }

        protected static TestLayoutVertex CreateTestLayoutVertex(string name, int priority = 1)
        {
            return new TestLayoutVertex(name, true, priority);
        }

        protected static TestDummyLayoutVertex CreateDummyLayoutVertex(string name)
        {
            return new TestDummyLayoutVertex(int.Parse(name.Substring(1)), true);
        }
    }
}
