using System.Collections.Generic;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Selectors
{
    /// <summary>
    /// Implements a DPI selector for image export.
    /// </summary>
    internal class DpiSelector : SyncCommandBase, ISelector<Dpi>
    {
        private static readonly IEnumerable<Dpi> DpiChoices = new List<Dpi> { Dpi.Dpi96, Dpi.Dpi150, Dpi.Dpi300, Dpi.Dpi600 };

        public DpiSelector(IAppServices appServices)
            : base(appServices)
        {
        }

        public IEnumerable<Dpi> GetAllItems() => DpiChoices;
        public Dpi GetSelectedItem() => UiService.ImageExportDpi;
        public void OnItemSelected(Dpi item) => UiService.ImageExportDpi = item;
    }
}
