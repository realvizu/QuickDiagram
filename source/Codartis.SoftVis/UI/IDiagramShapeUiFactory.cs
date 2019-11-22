using Codartis.SoftVis.Diagramming.Definition;
using Codartis.Util.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates diagram shape UI instances.
    /// </summary>
    public interface IDiagramShapeUiFactory
    {
        [CanBeNull] IPayloadUiFactory PayloadUiFactory { get; }

        [NotNull]
        IDiagramNodeUi CreateDiagramNodeUi(
            [NotNull] IDiagramNode diagramNode,
            [NotNull] IFocusTracker<IDiagramShapeUi> focusTracker);

        [NotNull]
        IDiagramConnectorUi CreateDiagramConnectorUi(
            [NotNull] IDiagramConnector diagramConnector,
            [NotNull] IFocusTracker<IDiagramShapeUi> focusTracker);
    }
}