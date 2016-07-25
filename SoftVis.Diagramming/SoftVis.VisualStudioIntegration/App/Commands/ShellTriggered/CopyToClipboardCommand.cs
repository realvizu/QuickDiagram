using System;
using System.Windows;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Copies the current diagram to the clipboard.
    /// </summary>
    internal sealed class CopyToClipboardCommand : ShellTriggeredCommandBase
    {
        public CopyToClipboardCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.CopyToClipboradCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            UiServices.GetDiagramImage(Clipboard.SetImage);
        }
    }
}
