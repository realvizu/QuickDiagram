using System;
using System.Windows;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class CopyToClipboardCommand : CommandBase
    {
        public CopyToClipboardCommand(IWindowManager windowManager, IServiceProvider serviceProvider)
            : base(Constants.MainMenuCommands, Constants.CopyToClipboradCommand, windowManager, serviceProvider)
        { }

        protected override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = _windowManager.GetDiagramWindow();
            var bitmap = diagramWindow.GetDiagramAsBitmap();
            Clipboard.SetImage(bitmap);
        }
    }
}
