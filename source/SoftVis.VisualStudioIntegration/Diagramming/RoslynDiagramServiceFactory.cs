using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Creates roslyn-based diagram service instances.
    /// </summary>
    internal class RoslynDiagramServiceFactory : IDiagramServiceFactory
    {
        public IDiagramService Create(IReadOnlyModelStore modelStore)
        {
            return new RoslynDiagramService(
                modelStore,
                new RoslynDiagramStore(new Diagram()),
                new RoslynDiagramShapeFactory());
        }
    }
}
