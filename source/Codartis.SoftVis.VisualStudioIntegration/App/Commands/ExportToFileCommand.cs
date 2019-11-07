using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.Util.UI.Wpf.Imaging;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Exports the current diagram to a file.
    /// </summary>
    [UsedImplicitly]
    internal sealed class ExportToFileCommand : DiagramImageCreatorCommandBase
    {
        public ExportToFileCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            var filename = HostUiService.SelectSaveFilename("Save Diagram Image to File", "PNG Image|*.png");
            if (string.IsNullOrWhiteSpace(filename))
                return;

            if (!filename.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
            {
                HostUiService.ShowMessageBox("Only PNG file format is supported. Please select a file with .png extension.");
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
