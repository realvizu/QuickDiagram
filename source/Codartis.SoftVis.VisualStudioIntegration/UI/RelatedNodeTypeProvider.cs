using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RelatedNodeTypeProvider : IRelatedNodeTypeProvider
    {
        [NotNull]
        private static readonly ModelNodeStereotype[] TypesWithMembersStereotypes =
        {
            ModelNodeStereotypes.Class,
            ModelNodeStereotypes.Interface,
            ModelNodeStereotypes.Struct,
            ModelNodeStereotypes.Enum
        };

        //[NotNull]
        //private static readonly ModelNodeStereotype[] MemberStereotypes =
        //{
        //    ModelNodeStereotypes.Event,
        //    ModelNodeStereotypes.Field,
        //    ModelNodeStereotypes.Method,
        //    ModelNodeStereotypes.Property
        //};

        public IEnumerable<RelatedNodeType> GetRelatedNodeTypes(ModelNodeStereotype stereotype)
        {
            //yield return new RelatedNodeType(CommonDirectedModelRelationshipTypes.Container, "Parent");
            
            if (stereotype.In(TypesWithMembersStereotypes))
            {
                yield return new RelatedNodeType(CommonDirectedModelRelationshipTypes.Contained, "Members");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.AssociatedType, "Outgoing Associations");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.AssociatingType, "Incoming Associations");
            }

            if (stereotype.Equals(ModelNodeStereotypes.Class))
            {
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.BaseType, "Base types");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.Subtype, "Subtypes");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.ImplementedInterface, "Interfaces");
            }
            else if (stereotype.Equals(ModelNodeStereotypes.Interface))
            {
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.BaseType, "Base types");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.Subtype, "Subtypes");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.ImplementerType, "Implementers");
            }
            else if (stereotype.Equals(ModelNodeStereotypes.Struct))
            {
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.ImplementedInterface, "Interfaces");
            }
        }
    }
}