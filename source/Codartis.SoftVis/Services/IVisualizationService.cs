using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services
{
    /// <summary>
    /// Provides the top level access point for model, diagram and diagram UI services.
    /// </summary>
    public interface IVisualizationService
    {
        [NotNull]
        IModelService GetModelService();

        DiagramId CreateDiagram();

        [NotNull]
        IDiagramService GetDiagramService(DiagramId diagramId);

        [NotNull]
        IDiagramUiService GetDiagramUiService(DiagramId diagramId);

        void DeleteDiagram(DiagramId diagramId);
    }
}