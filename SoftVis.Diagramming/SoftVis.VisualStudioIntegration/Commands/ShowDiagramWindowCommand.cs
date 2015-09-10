using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// Makes the diagram window visible.
    /// </summary>
    internal sealed class ShowDiagramWindowCommand : CommandBase
    {
        public ShowDiagramWindowCommand(IPackageServices packageServices)
            : base(VsctConstants.MainMenuCommands, VsctConstants.ShowDiagramWindowCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = PackageServices.GetDiagramWindow();
            diagramWindow.Show();
        }
    }
}
