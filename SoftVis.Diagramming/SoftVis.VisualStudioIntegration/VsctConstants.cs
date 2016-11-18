using System;

namespace Codartis.SoftVis.VisualStudioIntegration
{
    internal static class VsctConstants
    {
        internal static Guid SoftVisCommandSetGuid = new Guid("D45A1864-AB94-44FD-81CA-2602753B60D3");

        internal const int ToolWindowToolbar = 1;

        internal const int ShowDiagramWindowCommand = 100;
        internal const int AddToDiagramCommand = 101;
        internal const int AddToDiagramWithHierarchyCommand = 102;
        internal const int ClearDiagramCommand = 103;
        internal const int CopyToClipboradCommand = 106;
        internal const int ExportToFileCommand = 107;
        internal const int ImageDpiComboCommand = 108;
        internal const int ImageDpiComboGetItemsCommand = 109;

    }
}
