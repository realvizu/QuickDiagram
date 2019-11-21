using System;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.UI.Wpf.View;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Provides diagram UI services. Bundles the diagram control and its view model together.
    /// </summary>
    internal sealed class DiagramWindowService : WpfDiagramUiService, IDiagramWindowService
    {
        public double ExportedImageMargin { get; } = 10;
        public Dpi ImageExportDpi { get; set; }

        public DiagramWindowService(
            [NotNull] IDiagramUi diagramViewModel,
            [NotNull] DiagramControl diagramControl)
            : base(diagramViewModel, diagramControl)
        {
        }

        private RoslynDiagramViewModel RoslynDiagramViewModel => (RoslynDiagramViewModel)DiagramUi;

        public void ExpandAllNodes() => RoslynDiagramViewModel.ExpandAllNodes();
        public void CollapseAllNodes() => RoslynDiagramViewModel.CollapseAllNodes();

        public void ShowPopupMessage(string message, TimeSpan hideAfter = default) => RoslynDiagramViewModel.ShowPopupMessage(message, hideAfter);
    }
}