using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.ShellTriggered
{
    /// <summary>
    /// Exports the current diagram to a file.
    /// </summary>
    internal sealed class ExportToFileCommand : ShellTriggeredCommandBase
    {
        public ExportToFileCommand(IPackageServices packageServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ExportToFileCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            // TODO: choose filename, save
        }

        public static void SaveBitmapAsPng(BitmapSource bitmapSource, string filename)
        {
            var pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var stream = File.Create(filename))
            {
                pngBitmapEncoder.Save(stream);
            }
        }
    }
}
