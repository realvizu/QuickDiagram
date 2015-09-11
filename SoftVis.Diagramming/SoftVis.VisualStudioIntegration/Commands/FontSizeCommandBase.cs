using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// Base class for those commands that modify FontSize.
    /// </summary>
    internal abstract class FontSizeCommandBase : CommandBase
    {
        private const int MinFontSize = 6;
        private const int MaxFontSize = 24;

        protected FontSizeCommandBase(Guid commandSet, int commandId, IPackageServices packageServices) 
            : base(commandSet, commandId, packageServices)
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
