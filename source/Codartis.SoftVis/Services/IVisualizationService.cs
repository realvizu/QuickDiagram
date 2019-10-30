using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services
{
    /// <summary>
    /// Provides model, diagram and UI services.
    /// </summary>
    public interface IVisualizationService
    {
        DiagramId CreateDiagram();

        [NotNull]
        IModelService GetModelService();

        [NotNull]
        IDiagramService GetDiagramService(DiagramId diagramId);

        [NotNull]
        IUiService GetUiService(DiagramId diagramId);

        void RemoveDiagram(DiagramId diagramId);
    }
}