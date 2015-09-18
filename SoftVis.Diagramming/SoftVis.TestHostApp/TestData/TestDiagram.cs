using System.Linq;
using Codartis.SoftVis.Diagramming.Graph.Layout;
using Codartis.SoftVis.Diagramming.Graph.Layout.EfficientSugiyama;
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
                {
                    if (Nodes.Any(i => i.ModelEntity.Equals(relationship.Target)))
                        ShowConnector(relationship);
                }
            }

            Layout();
        }

        public void Layout()
        {
            var sugiyamaLayoutParameters = new SugiyamaLayoutParameters
            {
                LayoutDirection = LayoutDirection.SourcesAtTop,
                MinimizeEdgeLength = false,
                EdgeRoutingType = EdgeRoutingType.Straight,
                LayerDistance = 40,
            };
            Layout(LayoutType.Sugiyama, sugiyamaLayoutParameters);

            //Layout(LayoutType.Tree);
        }
    }
}
