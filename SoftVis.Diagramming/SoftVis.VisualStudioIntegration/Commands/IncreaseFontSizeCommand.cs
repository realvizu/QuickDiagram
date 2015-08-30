using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class IncreaseFontSizeCommand : CommandBase
    {
        public IncreaseFontSizeCommand(IWindowManager windowManager, IServiceProvider serviceProvider)
            : base(Constants.MainMenuCommands, Constants.IncreaseFontSizeCommand, windowManager, serviceProvider)
        { }

        protected override void Execute(object sender, EventArgs e)
        {
            _windowManager.IncreaseFontSize();
        }
    }
}
