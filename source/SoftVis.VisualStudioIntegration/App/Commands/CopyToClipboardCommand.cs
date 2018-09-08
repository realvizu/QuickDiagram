using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Copies the current diagram to the clipboard.
    /// </summary>
    [UsedImplicitly]
    internal sealed class CopyToClipboardCommand : DiagramImageCreatorCommandBase
    {
        public CopyToClipboardCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            await CreateAndProcessDiagramImageAsync(Clipboard.SetImage, "Adding image to clipboard...");
        }
    }
}
