using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Layout;
using Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama;
using Codartis.SoftVis.Graphs.Layout.VertexPlacement.SimplifiedSugiyama;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestDiagram : WpfDiagram
    {
        public readonly List<IModelItem> ModelItemsAddByClicks = new List<IModelItem>();

        public TestDiagram(IModel model)
        {
            foreach (var entity in model.Entities)
                ShowNode(entity);

            foreach (var relationship in model.Relationships)
                ModelItemsAddByClicks.Add(relationship);
        }

        protected override DiagramNode CreateDiagramNode(IModelEntity modelEntity)
        {
            var size = new Size2D(((TestModelEntity)modelEntity).Size, 30);
            return new DiagramNodeViewModel(modelEntity, Point2D.Zero, size);
        }

        public void Layout(int sweepNumber)
        {
            LayoutSimplifiedSugiyama(sweepNumber);
            //LayoutEfficientSugiyama();
            //LayoutTree();
        }

        private void LayoutSimplifiedSugiyama(int sweepNumber)
        {
            var sugiyamaLayoutParameters = new SimplifiedSugiyamaLayoutParameters()
            {
                EdgeRoutingType = EdgeRoutingType.Straight,
                VerticalGap = 40,
                HorizontalGap = 10,
                SweepNumber = sweepNumber
            };
            Layout(LayoutType.SimplifiedSugiyama, sugiyamaLayoutParameters);
        }

        private void LayoutEfficientSugiyama()
        {
            var sugiyamaLayoutParameters = new EfficientSugiyamaLayoutParameters
            {
                LayoutDirection = LayoutDirection.SourcesAtTop,
                MinimizeEdgeLength = false,
                EdgeRoutingType = EdgeRoutingType.Straight,
                LayerDistance = 40,
            };
            Layout(LayoutType.EfficientSugiyama, sugiyamaLayoutParameters);
        }

        private void LayoutTree()
        {
            Layout(LayoutType.Tree);
        }
    }
}
