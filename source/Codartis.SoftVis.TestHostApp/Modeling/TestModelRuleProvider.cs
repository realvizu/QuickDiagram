using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    public sealed class TestModelRuleProvider : IModelRuleProvider
    {
        [NotNull]
        private static readonly IDictionary<ModelRelationshipStereotype, (ModelNodeStereotype sourceNodeType, ModelNodeStereotype? targetNodeType)[]>
            ValidModelNodeTypesByRelationshipTypeMap = new Dictionary<ModelRelationshipStereotype, (ModelNodeStereotype, ModelNodeStereotype? )[]>
            {
                [ModelRelationshipStereotype.Containment] = new (ModelNodeStereotype, ModelNodeStereotype?)[]
                {
                    (ModelNodeStereotypes.Class, ModelNodeStereotypes.Property),
                    (ModelNodeStereotypes.Interface, ModelNodeStereotypes.Property)
                },
                [ModelRelationshipStereotypes.Association] = new (ModelNodeStereotype, ModelNodeStereotype?)[]
                {
                    (ModelNodeStereotypes.Property, ModelNodeStereotypes.Class),
                    (ModelNodeStereotypes.Property, ModelNodeStereotypes.Interface)
                },
                [ModelRelationshipStereotypes.Implementation] = new (ModelNodeStereotype, ModelNodeStereotype?)[]
                {
                    (ModelNodeStereotypes.Class, ModelNodeStereotypes.Interface)
                },
                [ModelRelationshipStereotypes.Inheritance] = new (ModelNodeStereotype, ModelNodeStereotype?)[]
                {
                    (ModelNodeStereotypes.Class, ModelNodeStereotypes.Class),
                    (ModelNodeStereotypes.Interface, ModelNodeStereotypes.Interface)
                },
            };

        public bool IsRelationshipTypeValid(ModelRelationshipStereotype modelRelationshipStereotype, IModelNode source, IModelNode target)
        {
            if (!ValidModelNodeTypesByRelationshipTypeMap.TryGetValue(modelRelationshipStereotype, out var validRelationshipTypes))
            {
                Debug.WriteLine($"No relationship validity info configured for {modelRelationshipStereotype}");
                return true;
            }

            return validRelationshipTypes.Contains((source.Stereotype, target.Stereotype)) ||
                   validRelationshipTypes.Contains((source.Stereotype, null));        }
    }
}