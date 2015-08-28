using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal abstract class CommandBase
    {
        private readonly Guid _commandSet;
        private readonly int _commandId;
        protected readonly IWindowManager _windowManager;
        protected readonly IServiceProvider _serviceProvider;

        protected CommandBase(Guid commandSet, int commandId, IWindowManager windowManager, IServiceProvider serviceProvider)
        {
            _commandSet = commandSet;
            _commandId = commandId;
            _windowManager = windowManager;
            _serviceProvider = serviceProvider;

            AddMenuCommand(_serviceProvider);
        }

        protected abstract void Execute(object sender, EventArgs e);

        private void AddMenuCommand(IServiceProvider serviceProvider)
        {
            var commandService = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(_commandSet, _commandId);
                var menuItem = new MenuCommand(Execute, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }
    }
}
