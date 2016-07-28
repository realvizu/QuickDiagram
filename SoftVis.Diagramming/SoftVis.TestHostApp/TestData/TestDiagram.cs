using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestDiagram : Diagram
    {
        public List<List<IModelItem>> ModelItemGroups { get; }

        public TestDiagram(TestModel model, IConnectorTypeResolver connectorTypeResolver) 
            : base(model, connectorTypeResolver)
        {
            ModelItemGroups = model.ItemGroups.ToList();
        }

        protected override Size2D CalculateDiagramNodeSize(IModelEntity modelEntity)
        {
            var height = 30;
            int nameAsInt;
            if (int.TryParse(modelEntity.Name, out nameAsInt))
                height = int.Parse(modelEntity.Name) % 4 * 5 + 25;

            return new Size2D(((TestModelEntity)modelEntity).Size, height);
        }
    }
}
