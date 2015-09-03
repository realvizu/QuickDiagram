using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class ExportToFileCommand : CommandBase
    {
        public ExportToFileCommand(IWindowManager windowManager, IServiceProvider serviceProvider)
            : base(Constants.MainMenuCommands, Constants.ExportToFileCommand, windowManager, serviceProvider)
        { }

        protected override void Execute(object sender, EventArgs e)
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
