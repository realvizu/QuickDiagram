using System;
using System.Linq;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Executed when the host requests the current value for the ImageDpiCombo, or the current value changes.
    /// </summary>
    internal sealed class ImageDpiComboCommand : ShellTriggeredCommandBase
    {
        public ImageDpiComboCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ImageDpiComboCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            switch (HostUiServices.GetComboCommandType(e))
            {
                case ComboCommandType.CurrentItemRequested:
                    var currentDpiName = UiServices.ImageExportDpi.Name;
                    HostUiServices.SetCurrentComboItem(e, currentDpiName);
                    break;

                case ComboCommandType.SelectedItemChanged:
                    var selectedString = HostUiServices.GetSelectedComboItem(e);
                    var selectedDpi = Dpi.DpiChoices.First(i => i.Name == selectedString);
                    UiServices.ImageExportDpi = selectedDpi;
                    break;
            }
        }
    }
}
