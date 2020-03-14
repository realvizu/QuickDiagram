using System;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Helpers;
using Codartis.Util.Ids;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders
{
    internal abstract class BuilderBase
    {
        private readonly ISequenceProvider _sequenceProvider = new SequenceGenerator();

        [NotNull]
        protected GeneralLayoutEdge CreateLayoutEdge([NotNull] EdgeSpecification edgeSpecification)
        {
            return CreateLayoutEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
        }

        [NotNull]
        protected GeneralLayoutEdge CreateLayoutEdge([NotNull] string sourceVertexName, [NotNull] string targetVertexName)
        {
            var source = CreateLayoutVertex(sourceVertexName);
            var target = CreateLayoutVertex(targetVertexName);
            return CreateLayoutEdge(source, target);
        }

        [NotNull]
        protected GeneralLayoutEdge CreateLayoutEdge([NotNull] LayoutVertexBase source, [NotNull] LayoutVertexBase target)
        {
            return new GeneralLayoutEdge(source, target, null);
        }

        [NotNull]
        protected LayoutVertexBase CreateLayoutVertex([NotNull] string name, int priority = 1)
        {
            return name.StartsWith("*", StringComparison.OrdinalIgnoreCase)
                ? (LayoutVertexBase)CreateDummyLayoutVertex(name)
                : CreateTestLayoutVertex(name, priority);
        }

        [NotNull]
        protected TestLayoutVertex CreateTestLayoutVertex([NotNull] string name, int priority = 1)
        {
            var id = _sequenceProvider.GetNext();
            return new TestLayoutVertex(id, name, priority);
        }

        [NotNull]
        protected static TestDummyLayoutVertex CreateDummyLayoutVertex([NotNull] string name)
        {
            return new TestDummyLayoutVertex(int.Parse(name.Substring(1)));
        }
    }
}