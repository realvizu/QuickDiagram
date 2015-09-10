using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// Increases the font size of the diagram.
    /// </summary>
    internal sealed class IncreaseFontSizeCommand : CommandBase
    {
        public IncreaseFontSizeCommand(IPackageServices packageServices)
            : base(VsctConstants.MainMenuCommands, VsctConstants.IncreaseFontSizeCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramWindow = PackageServices.GetDiagramWindow();
            diagramWindow.IncreaseFontSize();
        }
    }
}
