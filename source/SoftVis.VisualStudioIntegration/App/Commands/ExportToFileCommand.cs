using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util.UI.Wpf.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Exports the current diagram to a file.
    /// </summary>
    internal sealed class ExportToFileCommand : DiagramImageCreatorCommandBase
    {
        public ExportToFileCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            var filename = UiServices.SelectSaveFilename("Save Diagram Image to File", "PNG Image|*.png");
            if (string.IsNullOrWhiteSpace(filename))
                return;

            if (!filename.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
            {
                UiServices.ShowMessageBox("Only PNG file format is supported. Please select a file with .png extension.");
                return;
            }

            await CreateAndProcessDiagramImageAsync(i => SaveBitmapAsPng(i, filename), "Saving image file...");
        }

        private static void SaveBitmapAsPng(BitmapSource bitmapSource, string filename)
        {
            using (var fileStream = File.Create(filename))
                bitmapSource.ToPng(fileStream);
        }
    }
}
