using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.TestData
{
    class TestDiagram
    {
        public static Diagram Create(UmlModel umlModel)
        {
            var diagram = new Diagram();

            foreach(var element in umlModel)
            {
                diagram.ShowModelElement(element);

                var umlType = element as UmlType;
                if (umlType != null)
                {
                    foreach (var relationship in umlType.OutgoingRelationships)
                        diagram.ShowModelElement(relationship);
                }
            }

            diagram.Layout();

            return diagram;
        }
    }
}
