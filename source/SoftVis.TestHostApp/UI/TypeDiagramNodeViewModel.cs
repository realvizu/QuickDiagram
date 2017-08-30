using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.TestHostApp.UI
{
    internal class TypeDiagramNodeViewModel : DiagramNodeViewModelBase
    {
        public TypeDiagramNodeViewModel(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore, TypeDiagramNode diagramNode)
            : base(modelStore, diagramStore, diagramNode)
        {
        }

        public override object Clone()
        {
            return new TypeDiagramNodeViewModel(ModelStore, DiagramStore, TypeDiagramNode) { Size = Size };
        }

        public TypeDiagramNode TypeDiagramNode => (TypeDiagramNode)DiagramNode;
        public string Stereotype => $"<<{TypeDiagramNode.TypeNode.Stereotype.Name.ToLower()}>>";

        protected override IEnumerable<RelatedNodeType> GetRelatedNodeTypes()
        {
            yield return new RelatedNodeType(DirectedModelRelationshipTypes.BaseType, "Base types");
            yield return new RelatedNodeType(DirectedModelRelationshipTypes.Subtype, "Subtypes");
            yield return new RelatedNodeType(DirectedModelRelationshipTypes.ImplementerType, "Implementers");
            yield return new RelatedNodeType(DirectedModelRelationshipTypes.ImplementedInterface, "Interfaces");
        }
    }
}
