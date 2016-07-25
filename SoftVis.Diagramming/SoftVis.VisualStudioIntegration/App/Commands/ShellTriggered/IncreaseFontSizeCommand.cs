using System;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Increases the font size of the diagram.
    /// </summary>
    internal sealed class IncreaseFontSizeCommand : FontSizeCommandBase
    {
        public IncreaseFontSizeCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.IncreaseFontSizeCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            UiServices.FontSize = IncreaseFontSize(UiServices.FontSize);
        }
    }
}
