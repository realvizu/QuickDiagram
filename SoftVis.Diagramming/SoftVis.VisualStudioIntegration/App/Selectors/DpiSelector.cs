using System.Collections.Generic;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Selectors
{
    /// <summary>
    /// Implements a DPI selector for image export.
    /// </summary>
    internal class DpiSelector : CommandBase, ISelector<Dpi>
    {
        public DpiSelector(IAppServices appServices) 
            : base(appServices)
        {
        }

        public IEnumerable<Dpi> GetAllItems() => Dpi.DpiChoices;
        public Dpi GetSelectedItem() => UiServices.ImageExportDpi;
        public void OnItemSelected(Dpi item) => UiServices.ImageExportDpi = item;
    }
}
