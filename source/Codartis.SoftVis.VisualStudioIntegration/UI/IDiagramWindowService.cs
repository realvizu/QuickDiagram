using System;
using Codartis.SoftVis.UI.Wpf;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Extends the diagram UI service with diagram tool window specific operations.
    /// </summary>
    internal interface IDiagramWindowService : IWpfDiagramUiService
    {
        double ExportedImageMargin { get; }
        Dpi ImageExportDpi { get; set; }

        void ExpandAllNodes();
        void CollapseAllNodes();

        void ShowPopupMessage(string message, TimeSpan hideAfter = default);
    }
}