using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class DecreaseFontSizeCommand : CommandBase
    {
        public DecreaseFontSizeCommand(IWindowManager windowManager, IServiceProvider serviceProvider)
            : base(Constants.MainMenuCommands, Constants.DecreaseFontSizeCommand, windowManager, serviceProvider)
        { }

        protected override void Execute(object sender, EventArgs e)
        {
            _windowManager.DecreaseFontSize();
        }
    }
}
