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
        public IDiagramService Create(IModelService modelService)
        {
            return new RoslynDiagramService(
                Diagram.Empty,
                modelService, 
                new RoslynDiagramShapeFactory());
        }
    }
}
