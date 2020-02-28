using Codartis.SoftVis.Diagramming.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates diagram shape UI instances.
    /// </summary>
    public interface IDiagramShapeUiFactory
    {
        [NotNull]
        IDiagramNodeUi CreateDiagramNodeUi([NotNull] IDiagramNode diagramNode);

        [NotNull]
        IDiagramConnectorUi CreateDiagramConnectorUi([NotNull] IDiagramConnector diagramConnector);
    }
}