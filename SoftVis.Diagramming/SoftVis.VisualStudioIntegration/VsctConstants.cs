using System;

namespace Codartis.SoftVis.VisualStudioIntegration
{
    internal static class VsctConstants
    {
        internal static Guid CodeEditorContextMenuCommands = new Guid("3ec3e947-3047-4579-a09e-921b99ce5789");
        internal const int AddToDiagramCommand = 256;

        internal static Guid MainMenuCommands = new Guid("17f0822e-ef7f-40f7-8e1a-788dde2cd2b8");
        internal const int ShowDiagramWindowCommand = 256;
        internal const int ClearDiagramCommand = 257;
        internal const int IncreaseFontSizeCommand = 258;
        internal const int DecreaseFontSizeCommand = 259;
        internal const int CopyToClipboradCommand = 260;
        internal const int ExportToFileCommand = 261;

        internal static Guid ToolWindowToolbarCommands = new Guid("103E89FB-F029-4633-8482-B6435D4F35EF");
        internal const int ToolWindowToolbar = 4096;
    }
}
