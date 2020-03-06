using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    public sealed class ModelRelationshipFeatureProvider : IModelRelationshipFeatureProvider
    {
        [NotNull]
        private static readonly ModelRelationshipStereotype[] InheritanceGroup =
        {
            ModelRelationshipStereotypes.Inheritance,
            ModelRelationshipStereotypes.Implementation
        };

        public bool IsTransitive(ModelRelationshipStereotype stereotype) => stereotype.In(InheritanceGroup);

        public string GetTransitivityPartitionKey(ModelRelationshipStereotype stereotype)
        {
            return stereotype.In(InheritanceGroup)
                ? "Inheritance"
                : "Others";
        }
    }
}