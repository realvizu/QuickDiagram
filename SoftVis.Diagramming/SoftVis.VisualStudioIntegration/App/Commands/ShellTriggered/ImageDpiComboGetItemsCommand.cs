using System;
using System.Linq;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Executed when the host requests the items for ImageDpiCombo.
    /// </summary>
    internal sealed class ImageDpiComboGetItemsCommand : ShellTriggeredCommandBase
    {
        private static readonly string[] ComboItems;

        static ImageDpiComboGetItemsCommand()
        {
            ComboItems = ImageExport.Dpi.DpiChoices.Select(i => i.Name).ToArray();
        }

        public ImageDpiComboGetItemsCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ImageDpiComboGetItemsCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            HostUiServices.FillCombo(e, ComboItems);
        }
    }
}
