using System;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Clears the diagram.
    /// </summary>
    internal sealed class ClearDiagramCommand : ShellTriggeredCommandBase
    {
        public ClearDiagramCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ClearDiagramCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            UiServices.ShowDiagramWindow();
            DiagramServices.Clear();
        }
    }
}
