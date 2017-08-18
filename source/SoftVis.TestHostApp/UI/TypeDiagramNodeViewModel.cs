using System.Collections.Generic;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.TestHostApp.UI
{
    internal class TypeDiagramNodeViewModel : DiagramNodeViewModelBase
    {
        public TypeDiagramNode TypeDiagramNode { get; }
        public string Stereotype { get; }

        public TypeDiagramNodeViewModel(IArrangedDiagram diagram, TypeDiagramNode diagramNode)
            : this(diagram,  diagramNode, Size.Empty, PointExtensions.Undefined, PointExtensions.Undefined)
        {
        }

        public TypeDiagramNodeViewModel(IArrangedDiagram diagram, TypeDiagramNode diagramNode, Size size, Point center, Point topLeft)
            : base(diagram, diagramNode, size, center, topLeft)
        {
            TypeDiagramNode = diagramNode;
            Stereotype = $"<<{diagramNode.TypeNodeBase.Stereotype.Name.ToLower()}>>";
        }

        public override object Clone()
        {
            return new TypeDiagramNodeViewModel(Diagram, TypeDiagramNode, Size, Center, TopLeft);
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
