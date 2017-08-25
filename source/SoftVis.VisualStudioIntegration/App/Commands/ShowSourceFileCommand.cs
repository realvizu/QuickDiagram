using System;
using Codartis.SoftVis.Diagramming;

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
            var modelNode = diagramNode?.ModelNode;
            if (modelNode == null)
                throw new Exception("DiagramNode or ModelNode is null.");

            if (ModelServices.HasSource(modelNode))
                ModelServices.ShowSource(modelNode);
            else
                UiServices.ShowPopupMessage(NoSourceMessage, NoSourceMessageDuration);
        }
    }
}
