using System;
using System.Windows;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class CopyToClipboardCommand : CommandBase
    {
        public CopyToClipboardCommand(IHostServices hostServices)
            : base(VsctConstants.MainMenuCommands, VsctConstants.CopyToClipboradCommand, hostServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = HostServices.GetDiagramWindow();
            var bitmap = diagramWindow.GetDiagramAsBitmap();
            Clipboard.SetImage(bitmap);
        }
    }
}
