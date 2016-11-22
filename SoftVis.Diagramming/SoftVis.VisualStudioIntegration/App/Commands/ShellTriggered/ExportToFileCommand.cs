using System;
using System.IO;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util.UI.Wpf.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Exports the current diagram to a file.
    /// </summary>
    internal sealed class ExportToFileCommand : ShellTriggeredCommandBase
    {
        public ExportToFileCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ExportToFileCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var filename = UiServices.SelectSaveFilename("Save Diagram Image to File", "PNG Image|*.png");
            if (string.IsNullOrWhiteSpace(filename))
                return;

            if (filename.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
            {
                UiServices.GetDiagramImage(i => SaveBitmapAsPng(i, filename));
                return;
            }

            UiServices.MessageBox("Only PNG file format is supported. Please select a file with .png extension.");
        }

        private static void SaveBitmapAsPng(BitmapSource bitmapSource, string filename)
        {
            using (var fileStream = File.Create(filename))
                bitmapSource.ToPng(fileStream);
        }
    }
}
