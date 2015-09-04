using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class DecreaseFontSizeCommand : CommandBase
    {
        public DecreaseFontSizeCommand(IHostServices hostServices)
            : base(VsctConstants.MainMenuCommands, VsctConstants.DecreaseFontSizeCommand, hostServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = HostServices.GetDiagramWindow();
            diagramWindow.DecreaseFontSize();
        }
    }
}
