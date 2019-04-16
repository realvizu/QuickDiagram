using Codartis.SoftVis.Graphs.Concurrent;
using FluentAssertions;
using QuickGraph;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Graphs.Concurrent
{
    public class RemovedVertexResultTests
    {
        [Fact]
        public void EqualityComparison_ForEmptyResult_Works()
        {
            var emptyResult = new RemoveVertexResult<int,Edge<int>>();
            (emptyResult == RemoveVertexResult<int, Edge<int>>.Empty).Should().BeTrue();
        }
    }
}
