using System.Collections.Generic;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands;
using Codartis.SoftVis.VisualStudioIntegration.App.ToggleCommands;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.ComboAdapters;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Defines the commands that should be registered with the host shell.
    /// </summary>
    internal static class ShellCommands
    {
        public static readonly List<ICommandSpecification> CommandSpecifications =
            new List<ICommandSpecification>
            {
                new CommandSpecification<AddCurrentSymbolToDiagramCommand>(PackageIds.AddToDiagramCommand),
                new CommandSpecification<AddCurrentSymbolToDiagramWithHierarchyCommand>(PackageIds.AddToDiagramWithHierarchyCommand),
                new CommandSpecification<ClearModelCommand>(PackageIds.ClearModelCommand),
                new CommandSpecification<ClearDiagramCommand>(PackageIds.ClearDiagramCommand),
                new CommandSpecification<UpdateModelFromSourceCommand>(PackageIds.UpdateModelFromSourceCommand),
                new CommandSpecification<CopyToClipboardCommand>(PackageIds.CopyToClipboardCommand),
                new CommandSpecification<ExportToFileCommand>(PackageIds.ExportToFileCommand),
                new CommandSpecification<ShowDiagramWindowCommand>(PackageIds.ShowDiagramWindowCommand),
                new CommandSpecification<ZoomToDiagramCommand>(PackageIds.ZoomToDiagramCommand),
                // Individual expand/collapse is not supported at the moment.
                //new CommandSpecification<ExpandAllNodesCommand>(PackageIds.ExpandAllNodesCommand),
                //new CommandSpecification<CollapseAllNodesCommand>(PackageIds.CollapseAllNodesCommand),
                new CommandSpecification<ShowHideNodeDescriptionsToggleCommand>(PackageIds.ShowHideNodeDescriptionsCommand),
            };

        public static readonly List<IComboSpecification> ComboSpecifications =
            new List<IComboSpecification>
            {
                new ComboSpecification<DpiComboAdapter>(PackageIds.ImageDpiComboGetItemsCommand, PackageIds.ImageDpiComboCommand),
            };
    }
}