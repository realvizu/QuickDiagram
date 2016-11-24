using System;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Makes the diagram window visible.
    /// </summary>
    internal sealed class ShowDiagramWindowCommand : ShellTriggeredCommandBase
    {
        public ShowDiagramWindowCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ShowDiagramWindowCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            UiServices.ShowDiagramWindow();
        }
    }
}
