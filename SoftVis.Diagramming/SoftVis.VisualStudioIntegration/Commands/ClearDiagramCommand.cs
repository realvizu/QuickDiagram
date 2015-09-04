using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class ClearDiagramCommand : CommandBase
    {
        public ClearDiagramCommand(IHostServices hostServices)
            : base(VsctConstants.MainMenuCommands, VsctConstants.ClearDiagramCommand, hostServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = HostServices.GetDiagramWindow();
            diagramWindow.Show();
            diagramWindow.Clear();
        }
    }
}
