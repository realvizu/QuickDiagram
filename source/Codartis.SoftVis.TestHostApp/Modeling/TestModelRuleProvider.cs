using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    public sealed class TestModelRuleProvider : IModelRuleProvider
    {
        [NotNull]
        private static readonly IDictionary<ModelRelationshipStereotype, (ModelNodeStereotype sourceNodeType, ModelNodeStereotype targetNodeType)[]>
            ValidRelationshipTypes = new Dictionary<ModelRelationshipStereotype, (ModelNodeStereotype, ModelNodeStereotype )[]>
            {
                [ModelRelationshipStereotype.Containment] = new[]
                {
                    (ModelNodeStereotypes.Class, ModelNodeStereotypes.Property),
                    (ModelNodeStereotypes.Interface, ModelNodeStereotypes.Property)
                },
                [ModelRelationshipStereotypes.Association] = new[]
                {
                    (ModelNodeStereotypes.Property, ModelNodeStereotypes.Class),
                    (ModelNodeStereotypes.Property, ModelNodeStereotypes.Interface)
                },
                [ModelRelationshipStereotypes.Implementation] = new[]
                {
                    (ModelNodeStereotypes.Class, ModelNodeStereotypes.Interface)
                },
                [ModelRelationshipStereotypes.Inheritance] = new[]
                {
                    (ModelNodeStereotypes.Class, ModelNodeStereotypes.Class),
                    (ModelNodeStereotypes.Interface, ModelNodeStereotypes.Interface)
                },
            };

        public bool IsRelationshipStereotypeValid(ModelRelationshipStereotype modelRelationshipStereotype, IModelNode source, IModelNode target)
        {
            return ValidRelationshipTypes[modelRelationshipStereotype]?.Contains((source.Stereotype, target.Stereotype)) ?? false;
        }
    }
}