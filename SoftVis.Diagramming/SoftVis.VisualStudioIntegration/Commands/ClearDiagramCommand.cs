using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class ClearDiagramCommand : CommandBase
    {
        public ClearDiagramCommand(IWindowManager windowManager, IServiceProvider serviceProvider)
            : base(Constants.MainMenuCommands, Constants.ClearDiagramCommand, windowManager, serviceProvider)
        { }

        protected override void Execute(object sender, EventArgs e)
        {
            _windowManager.ShowDiagramWindow();
            DiagramBuilder.Instance.Clear();
        }
    }
}
