using System.Collections.Generic;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Selectors
{
    /// <summary>
    /// Non-generic marker interface for all kinds of selectors.
    /// </summary>
    internal interface ISelector
    { }

    /// <summary>
    /// Defines the operations of a selector.
    /// </summary>
    /// <typeparam name="T">The type of the items in the selector.</typeparam>
    internal interface ISelector<T> : ISelector
    {
        IEnumerable<T> GetAllItems();
        T GetSelectedItem();
        void OnItemSelected(T item);
    }
}
