using System.Collections.Generic;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.ComboAdapters;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Defines the commands that should be registered with the host shell.
    /// </summary>
    internal static class ShellCommands
    {
        public static readonly IEnumerable<ICommandSpecification> CommandSpecifications =
            new List<ICommandSpecification>
            {
                new CommandSpecification<AddToDiagramCommand>(VsctConstants.AddToDiagramCommand),
                new CommandSpecification<AddToDiagramWithHierarchyCommand>(VsctConstants.AddToDiagramWithHierarchyCommand),
                new CommandSpecification<ClearDiagramCommand>(VsctConstants.ClearDiagramCommand),
                new CommandSpecification<CopyToClipboardCommand>(VsctConstants.CopyToClipboardCommand),
                new CommandSpecification<ExportToFileCommand>(VsctConstants.ExportToFileCommand),
                new CommandSpecification<ShowDiagramWindowCommand>(VsctConstants.ShowDiagramWindowCommand),
            };

        public static readonly IEnumerable<IComboSpecification> ComboSpecifications =
            new List<IComboSpecification>
            {
                new ComboSpecification<DpiComboAdapter>(VsctConstants.ImageDpiComboGetItemsCommand, VsctConstants.ImageDpiComboCommand),
            };
    }
}