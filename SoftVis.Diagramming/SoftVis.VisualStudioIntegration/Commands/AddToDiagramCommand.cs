using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramCommand : CommandBase
    {
        public AddToDiagramCommand(IPackageServices packageServices)
            :base(VsctConstants.CodeEditorContextMenuCommands, VsctConstants.AddToDiagramCommand, packageServices)
        {
        }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = PackageServices.GetDiagramWindow();
            diagramWindow.Show();
            diagramWindow.AddCurrentSymbol();
        }
    }
}
