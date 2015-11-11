using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Implementers provide a view that is filtered to non-floating items.
    /// </summary>
    /// <typeparam name="T">The type that implements the interface.</typeparam>
    /// <typeparam name="TItem">The type of the underlying items to be filtered. Must implement IFloatable.</typeparam>
    internal interface IFilterableToNonFloating<out T, out TItem> : IFilterable<T, TItem>
        where TItem : IFloatable
    {
        T GetViewWithoutFloatingItems();
    }
}
