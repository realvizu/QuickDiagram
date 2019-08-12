using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI;

namespace Codartis.SoftVis.TestHostApp.UI
{
    internal class PropertyDiagramNodeViewModel : DiagramNodeViewModelBase
    {
        public PropertyDiagramNodeViewModel(
            IModelService modelService,
            IDiagramService diagramService,
            IFocusTracker<IDiagramShapeUi> focusTracker,
            PropertyDiagramNode diagramNode)
            : base(modelService, diagramService, focusTracker, diagramNode)
        {
        }

        public override object Clone() => new PropertyDiagramNodeViewModel(ModelService, DiagramService, FocusTracker, PropertyDiagramNode) { Size = Size };

        public PropertyDiagramNode PropertyDiagramNode => (PropertyDiagramNode)DiagramNode;

        protected override IEnumerable<RelatedNodeType> GetRelatedNodeTypes()
        {
            yield return new RelatedNodeType(DirectedModelRelationshipTypes.AssociatedType, "Type");
        }
    }
}