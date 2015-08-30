using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal static class Constants
    {
        internal static Guid CodeEditorContextMenuCommands = new Guid("3ec3e947-3047-4579-a09e-921b99ce5789");
        internal static int AddToDiagramCommand = 256;

        internal static Guid MainMenuCommands = new Guid("17f0822e-ef7f-40f7-8e1a-788dde2cd2b8");
        internal static int ShowDiagramWindowCommand = 256;
        internal static int ClearDiagramCommand = 257;
        internal static int IncreaseFontSizeCommand = 258;
        internal static int DecreaseFontSizeCommand = 259;

        internal static Guid ToolWindowToolbarCommands = new Guid("103E89FB-F029-4633-8482-B6435D4F35EF");
        internal static int ToolWindowToolbar = 4096;
    }
}
