using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.DiagramRendering;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// A diagram that is created from Roslyn based model and rendered with WPF.
    /// </summary>
    internal class RoslynBasedWpfDiagram : WpfDiagram
    {
        public RoslynBasedWpfDiagram(IConnectorTypeResolver connectorTypeResolver) 
            : base(connectorTypeResolver)
        {
        }
    }
}
