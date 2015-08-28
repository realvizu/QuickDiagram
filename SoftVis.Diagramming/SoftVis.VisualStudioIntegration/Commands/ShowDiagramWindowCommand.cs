using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class ShowDiagramWindowCommand : CommandBase
    {
        public ShowDiagramWindowCommand(IWindowManager windowManager, IServiceProvider serviceProvider)
            : base(Constants.MainMenuCommands, Constants.ShowDiagramWindowCommand, windowManager, serviceProvider)
        { }

        protected override void Execute(object sender, EventArgs e)
        {
            _windowManager.ShowDiagramWindow();
        }
    }
}
