using System;
using System.Collections.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    /// <summary>
    /// Hides the core IModelService with the concept of ItemGroups.
    /// </summary>
    internal interface ITestModelService
    {
        IImmutableList<IImmutableList<IModelNode>> ItemGroups { get; }

        //void AddItemToCurrentGroup(IModelNode modelNode);
        void StartNewGroup();

        event Action<ModelEventBase> ModelChanged;

        void AddNode([NotNull] ITestNode node, ITestNode parentNode = null);
        void RemoveNode([NotNull] ITestNode node);

        void AddRelationship([NotNull] IModelRelationship relationship);
        //void RemoveRelationship(ModelRelationshipId relationshipId);

        void ClearModel();

        [NotNull]
        IModelNode GetUnderlyingNode([NotNull] ITestNode node);

        [NotNull]
        ITestNode GetTestNodeByName([NotNull] string name);
    }
}