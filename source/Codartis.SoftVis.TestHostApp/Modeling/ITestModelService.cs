using System.Collections.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    /// <summary>
    /// Wraps the core IModelService and adds the concept of ItemGroups.
    /// </summary>
    internal interface ITestModelService
    {
        IImmutableList<IImmutableList<IModelNode>> ItemGroups { get; }

        void StartNewGroup();

        void AddNode([NotNull] ITestNode node, ITestNode parentNode = null);
        void RemoveNode(ModelNodeId nodeId);
        void AddRelationship(ModelNodeId sourceId, ModelNodeId targetId, ModelRelationshipStereotype? stereotype = null);
        void RemoveRelationship(ModelRelationshipId relationshipId);
        void ClearModel();

        [NotNull]
        IModelNode GetUnderlyingNode([NotNull] ITestNode node);

        Maybe<IModelNode> TryGetUnderlyingNodeByName([NotNull] string name);
        Maybe<ITestNode> TryGetTestNodeByName([NotNull] string name);
    }
}