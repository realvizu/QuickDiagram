using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class DiagramToolWindowCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("3ec3e947-3047-4579-a09e-921b99ce5789");

        private readonly IWindowManager _windowManager;

        public static DiagramToolWindowCommand Instance { get; private set; }

        private DiagramToolWindowCommand(IWindowManager windowManager, IServiceProvider serviceProvider)
        {
            _windowManager = windowManager;

            AddMenuCommand(serviceProvider);
        }

        public static void Initialize(IWindowManager windowManager, IServiceProvider serviceProvider)
        {
            Instance = new DiagramToolWindowCommand(windowManager, serviceProvider);
        }

        private void AddMenuCommand(IServiceProvider serviceProvider)
        {
            var commandService = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(AddToDiagram, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        private void AddToDiagram(object sender, EventArgs e)
        {
            _windowManager.ShowDiagramWindow();
            DiagramBuilder.Instance.AddCurrentSymbol();
        }
    }
}
