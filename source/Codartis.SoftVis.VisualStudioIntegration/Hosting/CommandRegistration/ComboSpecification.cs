using System;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.ComboAdapters;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration
{
    /// <summary>
    /// Describes a combo box with its command IDs and type.
    /// </summary>
    /// <typeparam name="T">The type of the combo adapter that translates between VS commands and the app logic.</typeparam>
    internal struct ComboSpecification<T> : IComboSpecification
        where T : IComboAdapter
    {
        public int GetItemsCommandId { get; }
        public int ComboCommandId { get; }

        public ComboSpecification(int getItemsCommandId, int comboCommandId)
        {
            GetItemsCommandId = getItemsCommandId;
            ComboCommandId = comboCommandId;
        }

        public Type ComboAdapterType => typeof(T);
    }
}
