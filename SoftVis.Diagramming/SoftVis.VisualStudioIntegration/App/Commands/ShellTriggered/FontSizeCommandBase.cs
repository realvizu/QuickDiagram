using System;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Base class for those commands that modify FontSize.
    /// </summary>
    internal abstract class FontSizeCommandBase : ShellTriggeredCommandBase
    {
        private const int MinFontSize = 6;
        private const int MaxFontSize = 24;

        protected FontSizeCommandBase(Guid commandSet, int commandId, IAppServices appServices) 
            : base(commandSet, commandId, appServices)
        {
        }

        protected static int IncreaseFontSize(int fontSize)
        {
            return Math.Min(fontSize + 1, MaxFontSize);
        }

        protected static int DecreaseFontSize(int fontSize)
        {
            return Math.Max(fontSize - 1, MinFontSize);
        }
    }
}
