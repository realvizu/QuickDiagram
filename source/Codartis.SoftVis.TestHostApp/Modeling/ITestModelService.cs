using System.Collections.Immutable;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal interface ITestModelService : IModelService, ITestModelMutator
    {
        IImmutableList<IImmutableList<IModelNode>> ItemGroups { get; }
    }
}