using System.Collections.Immutable;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal interface ITestModelService : IModelService, ITestModelStore
    {
        IImmutableList<IImmutableList<IModelNode>> ItemGroups { get; }
    }
}