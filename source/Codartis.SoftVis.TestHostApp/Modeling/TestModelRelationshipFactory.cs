using System;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelRelationshipFactory : IModelRelationshipFactory
    {
        public IModelRelationship CreateRelationship(IModelNode source, IModelNode target, ModelRelationshipStereotype stereotype)
        {
            var id = ModelRelationshipId.Create();

            if (stereotype == ModelRelationshipStereotype.Containment) return new ContainmentRelationship(id, source, target);
            if (stereotype == ModelRelationshipStereotypes.Implementation) return new ImplementationRelationship(id, source, target);
            if (stereotype == ModelRelationshipStereotypes.Inheritance) return new InheritanceRelationship(id, source, target);
            if (stereotype == ModelRelationshipStereotypes.Association) return new AssociationRelationship(id, source, target);

            throw new NotImplementedException($"Model relationship not implemented for stereotype {stereotype}");
        }
    }
}
