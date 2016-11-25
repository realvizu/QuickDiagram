using System.Windows;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Copies the current diagram to the clipboard.
    /// </summary>
    internal sealed class CopyToClipboardCommand : DiagramImageCommandBase
    {
        public CopyToClipboardCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            CreateDiagramImage(Clipboard.SetImage, "Adding image to clipboard...");
        }
    }
}
