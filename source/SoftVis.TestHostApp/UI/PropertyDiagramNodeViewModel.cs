using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.TestHostApp.UI
{
    internal class PropertyDiagramNodeViewModel : DiagramNodeViewModelBase
    {
        public PropertyDiagramNodeViewModel(IModelService modelService, IDiagramService diagramService, PropertyDiagramNode diagramNode)
            : base(modelService, diagramService, diagramNode)
        {
        }

        public override object Clone()
        {
            return new PropertyDiagramNodeViewModel(ModelService, DiagramService, PropertyDiagramNode) { Size = Size };
        }

        public PropertyDiagramNode PropertyDiagramNode => (PropertyDiagramNode)DiagramNode;

        protected override IEnumerable<RelatedNodeType> GetRelatedNodeTypes()
        {
            yield return new RelatedNodeType(DirectedModelRelationshipTypes.AssociatedType, "Type");
        }
    }
}
