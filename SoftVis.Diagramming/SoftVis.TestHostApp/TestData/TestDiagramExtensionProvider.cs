using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestDiagramExtensionProvider : DefaultDiagramExtensionProvider
    {
        private static readonly ConnectorStyle ImplementationConnectorStyle = new ConnectorStyle(ArrowHeadType.Hollow, LineType.Dashed);

        private static readonly RelatedEntityMiniButtonDescriptor BaseTypes =
            new RelatedEntityMiniButtonDescriptor(
                ModelRelationshipDirection.Outgoing, ModelRelationshipType.Generalization, null,
                new RectRelativeLocation(RectReferencePoint.TopCenter, new Point2D(-MiniButtonRadius * 1.2, MiniButtonOverlapParentBy)),
                BuiltInConnectorStyles.Generalization);

        private static readonly RelatedEntityMiniButtonDescriptor ImplementedInterfaces =
            new RelatedEntityMiniButtonDescriptor(
                ModelRelationshipDirection.Outgoing, ModelRelationshipType.Generalization, TestModelRelationshipStereotype.Implementation,
                new RectRelativeLocation(RectReferencePoint.TopCenter, new Point2D(MiniButtonRadius * 1.2, MiniButtonOverlapParentBy)),
                ImplementationConnectorStyle);

        public override IEnumerable<RelatedEntityMiniButtonDescriptor> GetRelatedEntityMiniButtonDescriptors()
        {
            yield return BaseTypes;
            yield return ImplementedInterfaces;
        }

        public override ConnectorStyle GetConnectorStyle(IModelRelationship modelRelationship)
        {
            return modelRelationship.Stereotype == TestModelRelationshipStereotype.Implementation
                ? ImplementationConnectorStyle
                : BuiltInConnectorStyles.Generalization;
        }
    }
}
