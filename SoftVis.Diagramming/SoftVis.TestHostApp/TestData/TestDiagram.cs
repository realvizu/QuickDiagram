using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestDiagram : WpfDiagram
    {
        public TestDiagram(IModel model)
        {
            foreach (var entity in model.Entities)
            {
                ShowNode(entity);

                foreach (var relationship in entity.OutgoingRelationships)
                    ShowConnector(relationship);
            }

            LayoutNodes();
            RouteConnectors();
        }
    }
}
