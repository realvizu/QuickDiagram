using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class IncreaseFontSizeCommand : CommandBase
    {
        public IncreaseFontSizeCommand(IHostServices hostServices)
            : base(VsctConstants.MainMenuCommands, VsctConstants.IncreaseFontSizeCommand, hostServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = HostServices.GetDiagramWindow();
            diagramWindow.IncreaseFontSize();
        }
    }
}
