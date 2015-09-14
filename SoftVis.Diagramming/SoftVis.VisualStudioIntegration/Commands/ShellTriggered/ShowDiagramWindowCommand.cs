using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.ShellTriggered
{
    /// <summary>
    /// Makes the diagram window visible.
    /// </summary>
    internal sealed class ShowDiagramWindowCommand : ShellTriggeredCommandBase
    {
        public ShowDiagramWindowCommand(IPackageServices packageServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ShowDiagramWindowCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var diagramServices = PackageServices.GetDiagramServices();
            diagramServices.ShowDiagram();
        }
    }
}
