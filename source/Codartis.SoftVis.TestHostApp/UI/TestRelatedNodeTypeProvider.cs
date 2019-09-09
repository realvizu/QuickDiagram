using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI;

namespace Codartis.SoftVis.TestHostApp.UI
{
    public sealed class TestRelatedNodeTypeProvider : IRelatedNodeTypeProvider
    {
        public IEnumerable<RelatedNodeType> GetRelatedNodeTypes(ModelNodeStereotype stereotype)
        {
            if (stereotype.Equals(ModelNodeStereotypes.Property))
            {
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.AssociatedType, "Type");
            }
            else if (stereotype.Equals(ModelNodeStereotypes.Class) ||
                     stereotype.Equals(ModelNodeStereotypes.Interface))
            {
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.BaseType, "Base types");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.Subtype, "Subtypes");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.ImplementerType, "Implementers");
                yield return new RelatedNodeType(DirectedModelRelationshipTypes.ImplementedInterface, "Interfaces");
            }
        }
    }
}