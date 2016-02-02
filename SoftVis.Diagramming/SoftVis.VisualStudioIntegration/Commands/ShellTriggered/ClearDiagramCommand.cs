using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.ShellTriggered
{
    /// <summary>
    /// Clears the diagram.
    /// </summary>
    internal sealed class ClearDiagramCommand : ShellTriggeredCommandBase
    {
        public ClearDiagramCommand(IPackageServices packageServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ClearDiagramCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var uiServices = PackageServices.GetUIServices();
            uiServices.ShowDiagramWindow();

            var diagramServices = PackageServices.GetDiagramServices();
            diagramServices.ClearDiagram();
        }
    }
}
