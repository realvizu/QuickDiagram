using System;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Defines interaction points for the app logic and the UI.
    /// </summary>
    public interface IDiagramUi
    {
        event Action<IDiagramNode> DiagramNodeInvoked;
    }
}
