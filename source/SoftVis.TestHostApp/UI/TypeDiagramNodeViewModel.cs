using System.Collections.Generic;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.TestHostApp.UI
{
    internal class TypeDiagramNodeViewModel : DiagramNodeViewModelBase
    {
        public TypeDiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode, bool isDescriptionVisible)
            : this(diagram,  diagramNode,  isDescriptionVisible, Size.Empty, PointExtensions.Undefined, PointExtensions.Undefined)
        {
        }

        public TypeDiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode, bool isDescriptionVisible,
            Size size, Point center, Point topLeft)
            : base(diagram, diagramNode, isDescriptionVisible, size, center, topLeft)
        {
        }

        public override object Clone()
        {
            return new TypeDiagramNodeViewModel(Diagram, DiagramNode, IsDescriptionVisible, Size, Center, TopLeft);
        }

        protected override IEnumerable<RelatedNodeType> GetRelatedNodeTypes()
        {
            yield return new RelatedNodeType(DirectedRelationshipTypes.BaseType, "Base types");
            yield return new RelatedNodeType(DirectedRelationshipTypes.Subtype, "Subtypes");
            yield return new RelatedNodeType(DirectedRelationshipTypes.ImplementerType, "Implementers");
            yield return new RelatedNodeType(DirectedRelationshipTypes.ImplementedInterface, "Interfaces");
        }
    }
}
