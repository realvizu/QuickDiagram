using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Activates the source code editor window for a given Symbol.
    /// </summary>
    internal class ShowSourceFileCommand : SyncCommandWithParameterBase<IDiagramNode>
    {
        private const string NoSourceMessage = "There's no source file for this item.";
        private static readonly TimeSpan NoSourceMessageDuration = TimeSpan.FromSeconds(5);

        public ShowSourceFileCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override void Execute(IDiagramNode diagramNode)
        {
            var roslynModelNode = diagramNode?.ModelNode as IRoslynModelNode;
            if (roslynModelNode == null)
                throw new Exception("DiagramNode or ModelNode is null or not an IRoslynModelNode.");

            if (ModelService.HasSource(roslynModelNode))
                ModelService.ShowSource(roslynModelNode);
            else
                UiService.ShowPopupMessage(NoSourceMessage, NoSourceMessageDuration);
        }
    }
}
