using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal sealed class AddToDiagramCommand : CommandBase
    {
        public AddToDiagramCommand(IWindowManager windowManager, IServiceProvider serviceProvider)
            : base(Constants.CodeEditorContextMenuCommands, Constants.AddToDiagramCommand, windowManager, serviceProvider)
        { }

        protected override void Execute(object sender, EventArgs e)
        {
            _windowManager.ShowDiagramWindow();
            DiagramBuilder.Instance.AddCurrentSymbol();
        }
    }
}
