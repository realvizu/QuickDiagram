using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Implementation
{
    public abstract class ModelRuleProviderBase : IModelRuleProvider
    {
        public bool IsRelationshipStereotypeValid(ModelRelationshipStereotype modelRelationshipStereotype, IModelNode source, IModelNode target)
        {
            return GetValidSourceAndTargetNodeTypePairs(modelRelationshipStereotype).Contains((source.Stereotype, target.Stereotype));
        }

        [NotNull]
        protected abstract IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs(
            ModelRelationshipStereotype modelRelationshipStereotype);
    }
}