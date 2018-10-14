using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Codartis.SoftVis.VisualStudioIntegration.App.Selectors;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.ComboAdapters
{
    /// <summary>
    /// Translates between DpiSelector and its combo box implementation.
    /// </summary>
    internal class DpiComboAdapter : ComboAdapter<Dpi>
    {
        private readonly string[] _comboItems;

        public DpiComboAdapter(IAppServices appServices) 
            : base(new DpiSelector(appServices))
        {
            _comboItems = Selector.GetAllItems().Select(i => i.Name).ToArray();
        }

        protected override IEnumerable<string> GetComboItems()
        {
            return _comboItems;
        }

        protected override string GetSelectedItem()
        {
            return Selector.GetSelectedItem().Name;
        }

        protected override void OnSelectedItemChanged(string item)
        {
            var selectedDpi = Selector.GetAllItems().First(i => i.Name == item);
            Selector.OnItemSelected(selectedDpi);
        }
    }
}
