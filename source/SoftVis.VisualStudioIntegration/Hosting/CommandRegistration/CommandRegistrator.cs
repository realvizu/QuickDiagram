using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands;
using Codartis.SoftVis.VisualStudioIntegration.App.ToggleCommands;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.ComboAdapters;
using Microsoft.VisualStudio.Shell;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration
{
    /// <summary>
    /// Implements logic to register commands with the VS host.
    /// </summary>
    internal class CommandRegistrator
    {
        private readonly IMenuCommandService _menuCommandService;
        private readonly IAppServices _appServices;

        public CommandRegistrator(IMenuCommandService menuCommandService, IAppServices appServices)
        {
            _menuCommandService = menuCommandService;
            _appServices = appServices;
        }

        public void RegisterCommands(Guid commandSetGuid, List<ICommandSpecification> commandSpecifications)
        {
            RegisterSyncCommands(commandSetGuid, commandSpecifications.Where(i => i.CommandType.IsSubclassOf(typeof(SyncCommandBase))));
            RegisterAsyncCommands(commandSetGuid, commandSpecifications.Where(i => i.CommandType.IsSubclassOf(typeof(AsyncCommandBase))));
            RegisterToggleCommands(commandSetGuid, commandSpecifications.Where(i => i.CommandType.IsSubclassOf(typeof(ToggleCommandBase))));
        }

        public void RegisterCombos(Guid commandSetGuid, IEnumerable<IComboSpecification> comboSpecifications)
        {
            foreach (var comboSpecification in comboSpecifications)
            {
                var comboAdapter = (IComboAdapter)Activator.CreateInstance(comboSpecification.ComboAdapterType, _appServices);
                AddMenuCommand(commandSetGuid, comboSpecification.GetItemsCommandId, comboAdapter.GetItemsCommandHandler);
                AddMenuCommand(commandSetGuid, comboSpecification.ComboCommandId, comboAdapter.ComboCommandHandler);
            }
        }

        private void RegisterSyncCommands(Guid commandSetGuid, IEnumerable<ICommandSpecification> commandSpecifications)
        {
            foreach (var commandSpecification in commandSpecifications)
            {
                var command = (SyncCommandBase)Activator.CreateInstance(commandSpecification.CommandType, _appServices);

                AddMenuCommand(
                    commandSetGuid, 
                    commandSpecification.CommandId, 
                    (o, e) => command.Execute(),
                    CreateCommandEnablerHandler(command.IsEnabled));
            }
        }

        private void RegisterAsyncCommands(Guid commandSetGuid, IEnumerable<ICommandSpecification> commandSpecifications)
        {
            foreach (var commandSpecification in commandSpecifications)
            {
                var command = (AsyncCommandBase)Activator.CreateInstance(commandSpecification.CommandType, _appServices);

                AddMenuCommand(
                    commandSetGuid, 
                    commandSpecification.CommandId, 
                    (o, e) => command.ExecuteAsync(), 
                    CreateCommandEnablerHandler(command.IsEnabled));
            }
        }

        private static EventHandler CreateCommandEnablerHandler(Func<bool> isCommandEnabledPredicate)
        {
            return (sender, e) =>
            {
                var menuCommand = sender as OleMenuCommand;
                if (menuCommand != null && isCommandEnabledPredicate != null)
                    menuCommand.Visible = isCommandEnabledPredicate.Invoke();
            };
        }

        private void RegisterToggleCommands(Guid commandSetGuid, IEnumerable<ICommandSpecification> commandSpecifications)
        {
            foreach (var commandSpecification in commandSpecifications)
            {
                var command = (ToggleCommandBase)Activator.CreateInstance(commandSpecification.CommandType, _appServices);

                AddMenuCommand(
                    commandSetGuid,
                    commandSpecification.CommandId,
                    (o, e) => command.Execute(),
                    CreateCommandToggleHandler(command));
            }
        }

        private static EventHandler CreateCommandToggleHandler(ToggleCommandBase toggleCommand)
        {
            return (sender, e) =>
            {
                var menuCommand = sender as OleMenuCommand;
                if (menuCommand != null && toggleCommand != null)
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
