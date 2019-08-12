using System.Collections.Immutable;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal interface ITestModelService : IModelService
    {
        IImmutableList<IImmutableList<IModelNode>> ItemGroups { get; }

        void AddItemToCurrentGroup(IModelNode modelNode);
        void StartNewGroup();    }
}