using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class AddToDiagramCommand : CommandBase
    {
        public AddToDiagramCommand(IHostServices hostServices)
            :base(VsctConstants.CodeEditorContextMenuCommands, VsctConstants.AddToDiagramCommand, hostServices)
        {
        }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = HostServices.GetDiagramWindow();
            diagramWindow.Show();
            diagramWindow.AddCurrentSymbol();
        }
    }
}
