using System;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Decreases the font size of the diagram.
    /// </summary>
    internal sealed class DecreaseFontSizeCommand : FontSizeCommandBase
    {
        public DecreaseFontSizeCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.DecreaseFontSizeCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            UiServices.FontSize = DecreaseFontSize(UiServices.FontSize);
        }
    }
}
