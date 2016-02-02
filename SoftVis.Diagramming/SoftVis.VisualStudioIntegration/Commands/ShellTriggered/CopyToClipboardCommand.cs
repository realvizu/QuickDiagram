using System;
using System.Windows;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.ShellTriggered
{
    /// <summary>
    /// Copies the current diagram to the clipboard.
    /// </summary>
    internal sealed class CopyToClipboardCommand : ShellTriggeredCommandBase
    {
        public CopyToClipboardCommand(IPackageServices packageServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.CopyToClipboradCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var uiServices = PackageServices.GetUIServices();
            uiServices.GetDiagramImage(Clipboard.SetImage);
        }
    }
}
