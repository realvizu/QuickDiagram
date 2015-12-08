using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Shapes;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestDiagram : WpfDiagram
    {
        public List<List<IModelItem>> ModelItemGroups { get; }

        public TestDiagram(IConnectorTypeResolver connectorTypeResolver, TestModel model) 
            : base(connectorTypeResolver)
        {
            ModelItemGroups = model.ItemGroups.ToList();
        }

        protected override DiagramNode CreateDiagramNode(IModelEntity modelEntity)
        {
            var height = 30;
            int nameAsInt;
            if (int.TryParse(modelEntity.Name, out nameAsInt))
                height = (int.Parse(modelEntity.Name) % 4) * 5 + 25;

            var size = new Size2D(((TestModelEntity)modelEntity).Size, height);
            return new DiagramNodeViewModel(modelEntity, Point2D.Empty, size);
        }
    }
}
