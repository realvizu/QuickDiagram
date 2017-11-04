using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.TestHostApp.UI
{
    public class TestDiagramShapeUiFactory : DiagramShapeUiFactoryBase
    {
        public override IDiagramNodeUi CreateDiagramNodeUi(IDiagramService diagramService, IDiagramNode diagramNode)
        {
            if (diagramNode is TypeDiagramNode typeDiagramNode)
                return new TypeDiagramNodeViewModel(ModelService, diagramService, typeDiagramNode);

            if (diagramNode is PropertyDiagramNode propertyDiagramNode)
                return new PropertyDiagramNodeViewModel(ModelService, diagramService, propertyDiagramNode);

            throw new ArgumentException($"Unexpected type {diagramNode.GetType().Name} in {GetType().Name}");
        }
    }
}