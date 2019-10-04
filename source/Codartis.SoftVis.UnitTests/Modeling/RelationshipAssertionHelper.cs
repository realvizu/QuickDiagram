using Codartis.SoftVis.Modeling.Definition;
using FluentAssertions;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UnitTests.Modeling
{
    public static class RelationshipAssertionHelper
    {
        public static void ShouldMatch(
            [NotNull] this IModelRelationship relationship,
            ModelNodeId expectedSourceId,
            ModelNodeId expectedTargetId,
            ModelRelationshipStereotype expectedStereotype)
        {
            relationship.Stereotype.Should().Be(expectedStereotype);
            relationship.Source.Should().Be(expectedSourceId);
            relationship.Target.Should().Be(expectedTargetId);
        }
    }
}