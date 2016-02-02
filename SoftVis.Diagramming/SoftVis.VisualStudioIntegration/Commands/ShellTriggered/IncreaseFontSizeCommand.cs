using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.ShellTriggered
{
    /// <summary>
    /// Increases the font size of the diagram.
    /// </summary>
    internal sealed class IncreaseFontSizeCommand : FontSizeCommandBase
    {
        public IncreaseFontSizeCommand(IPackageServices packageServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.IncreaseFontSizeCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var uiServices = PackageServices.GetUIServices();
            uiServices.FontSize = IncreaseFontSize(uiServices.FontSize);
        }
    }
}
