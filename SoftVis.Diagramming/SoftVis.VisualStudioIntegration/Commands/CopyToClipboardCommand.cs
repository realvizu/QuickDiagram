using System;
using System.Windows;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// Copies the current diagram to the clipboard.
    /// </summary>
    internal sealed class CopyToClipboardCommand : CommandBase
    {
        public CopyToClipboardCommand(IPackageServices packageServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.CopyToClipboradCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = PackageServices.GetDiagramWindow();
            var bitmap = diagramWindow.GetDiagramAsBitmap();
            Clipboard.SetImage(bitmap);
        }
    }
}
