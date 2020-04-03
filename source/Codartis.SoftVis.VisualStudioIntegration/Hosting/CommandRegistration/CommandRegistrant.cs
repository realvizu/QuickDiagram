using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Autofac;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands.Toggle;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.ComboAdapters;
using Microsoft.VisualStudio.Shell;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration
{
    /// <summary>
    /// Implements logic to register commands with the VS host.
    /// </summary>
    internal sealed class CommandRegistrant
    {
        private readonly IMenuCommandService _menuCommandService;
        private readonly IContainer _container;

        public CommandRegistrant(IMenuCommandService menuCommandService, IContainer container)
        {
            _menuCommandService = menuCommandService;
            _container = container;
        }

        public void RegisterCommands(Guid commandSetGuid, IEnumerable<ICommandSpecification> commandSpecifications)
        {
            foreach (var commandSpecification in commandSpecifications)
            {
                var command = (CommandBase)_container.Resolve(commandSpecification.CommandType);

                AddMenuCommand(
                    commandSetGuid,
                    commandSpecification.CommandId,
                    (o, e) => command.ExecuteAsync(),
                    CreateCommandEnablerHandler(() => command.IsEnabledAsync()));
            }
        }

        public void RegisterToggleCommands(Guid commandSetGuid, IEnumerable<ICommandSpecification> commandSpecifications)
        {
            foreach (var commandSpecification in commandSpecifications)
            {
                var command = (ToggleCommandBase) _container.Resolve(commandSpecification.CommandType);

                AddMenuCommand(
                    commandSetGuid,
                    commandSpecification.CommandId,
#pragma warning disable 4014
                    (o, e) => command.ExecuteAsync(),
#pragma warning restore 4014
                    CreateCommandToggleHandler(command));
            }
        }

        public void RegisterCombos(Guid commandSetGuid, IEnumerable<IComboSpecification> comboSpecifications)
        {
            foreach (var comboSpecification in comboSpecifications)
            {
                var comboAdapter = (IComboAdapter) _container.Resolve(comboSpecification.ComboAdapterType);
                AddMenuCommand(commandSetGuid, comboSpecification.GetItemsCommandId, comboAdapter.GetItemsCommandHandler);
                AddMenuCommand(commandSetGuid, comboSpecification.ComboCommandId, comboAdapter.ComboCommandHandler);
            }
        }

        private static EventHandler CreateCommandEnablerHandler(Func<Task<bool>> asyncIsCommandEnabledPredicate)
        {
            return (sender, e) =>
            {
                if (sender is OleMenuCommand menuCommand && asyncIsCommandEnabledPredicate != null)
                    menuCommand.Visible = ThreadHelper.JoinableTaskFactory.Run(asyncIsCommandEnabledPredicate);
            };
        }

        private static EventHandler CreateCommandToggleHandler(ToggleCommandBase toggleCommand)
        {
            return (sender, e) =>
            {
                if (sender is OleMenuCommand menuCommand && toggleCommand != null)
                    menuCommand.Checked = toggleCommand.IsChecked;
            };
        }

        private void AddMenuCommand(Guid commandSet, int commandId, EventHandler commandHandler, EventHandler beforeQueryStatusHandler = null)
        {
            var menuCommandId = new CommandID(commandSet, commandId);
            var menuCommand = new OleMenuCommand(commandHandler, menuCommandId);
            menuCommand.BeforeQueryStatus += beforeQueryStatusHandler;

            _menuCommandService.AddCommand(menuCommand);
        }
    }
}