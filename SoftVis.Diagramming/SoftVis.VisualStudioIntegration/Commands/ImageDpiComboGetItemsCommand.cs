using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// Executed when the host requests the items for ImageDpiCombo.
    /// </summary>
    internal sealed class ImageDpiComboGetItemsCommand : CommandBase
    {
        private static readonly string[] ComboItems;

        static ImageDpiComboGetItemsCommand()
        {
            ComboItems = ImageExport.Dpi.DpiChoices.Select(i => i.Name).ToArray();
        }

        public ImageDpiComboGetItemsCommand(IPackageServices packageServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ImageDpiComboGetItemsCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var oleMenuCmdEventArgs = (OleMenuCmdEventArgs)e;

            var outValue = oleMenuCmdEventArgs.OutValue;
            if (outValue != IntPtr.Zero)
                Marshal.GetNativeVariantForObject(ComboItems, outValue);
        }
    }
}
