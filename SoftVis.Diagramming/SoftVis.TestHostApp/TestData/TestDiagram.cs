using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestDiagram
    {
        public static Diagram Create(UmlModel umlModel)
        {
            var diagram = new Diagram();

            foreach(var element in umlModel)
            {
                diagram.ShowNode(element);

                var umlType = element as UmlType;
                if (umlType != null)
                {
                    foreach (var relationship in umlType.OutgoingRelationships)
                        diagram.ShowConnector(relationship);
                }
            }

            diagram.Layout();

            return diagram;
        }
    }
}
