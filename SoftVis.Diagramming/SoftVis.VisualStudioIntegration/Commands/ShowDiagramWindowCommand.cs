using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class ShowDiagramWindowCommand : CommandBase
    {
        public ShowDiagramWindowCommand(IHostServices hostServices)
            : base(VsctConstants.MainMenuCommands, VsctConstants.ShowDiagramWindowCommand, hostServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = HostServices.GetDiagramWindow();
            diagramWindow.Show();
        }
    }
}
