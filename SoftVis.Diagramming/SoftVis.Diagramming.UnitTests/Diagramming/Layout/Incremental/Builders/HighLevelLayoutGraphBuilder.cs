using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class HighLevelLayoutGraphBuilder : GraphBuilderBase<DiagramNodeLayoutVertex, LayoutPath, HighLevelLayoutGraph>
    {
        protected override IEnumerable<string> PathSpecificationToVertexNames(string pathSpecification)
        {
            return base.PathSpecificationToVertexNames(pathSpecification).Where(i => !i.StartsWith("*"));
        }

        protected override DiagramNodeLayoutVertex CreateGraphVertex(string name)
        {
            return CreateTestLayoutVertex(name);
        }

        protected override LayoutPath CreateGraphEdge(DiagramNodeLayoutVertex source, DiagramNodeLayoutVertex target)
        {
            return new LayoutPath(new LayoutEdge(source, target, null));
        }
    }
}