using System;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Defines the UI operations of the diagram control.
    /// </summary>
    public interface IUiServices
    {
        Dpi ImageExportDpi { get; set; }

        void FitDiagramToView();
        void GetDiagramImage(Action<BitmapSource> imageCreatedCallback);

        void MessageBox(string message);

        void ShowProgressWindow(string text, Action cancelAction, double progress = 0);
        void SetProgress(double progress);
        void HideProgressWindow();

        string SelectSaveFilename(string title, string filter);
    }
}
