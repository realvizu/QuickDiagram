using System.Linq;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic;
using Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Helpers;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders
{
    internal class LayeredLayoutGraphBuilder : GraphBuilderBase<DiagramNodeLayoutVertex, LayoutPath, LayeredLayoutGraph>
    {
        protected override PathSpecification GetPathSpecification(string pathString)
        {
            var originalPathSpecification = base.GetPathSpecification(pathString);
            var nonDummyVertices = originalPathSpecification.Where(i => !i.StartsWith("*"));
            return new PathSpecification(nonDummyVertices);
        }

        protected override DiagramNodeLayoutVertex CreateVertex(string name, int priority = 1)
        {
            return CreateTestLayoutVertex(name, priority);
        }

        protected override LayoutPath CreateEdge(DiagramNodeLayoutVertex source, DiagramNodeLayoutVertex target)
        {
            return new LayoutPath(source, target, null);
        }
    }
}