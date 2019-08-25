using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Defines the rules that models must adhere to.
    /// </summary>
    public interface IModelRuleProvider
    {
        bool IsRelationshipStereotypeValid(ModelRelationshipStereotype modelRelationshipStereotype, [NotNull] IModelNode source, [NotNull] IModelNode target);
    }
}