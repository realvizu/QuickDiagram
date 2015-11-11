using System;

namespace Codartis.SoftVis.Common
{
    /// <summary>
    /// Implementers provide the ability to get a filtered view of a collection object 
    /// using a predicate on the underlying items.
    /// </summary>
    /// <typeparam name="T">The type that implements the interface.</typeparam>
    /// <typeparam name="TItem">The type of the underlying items to be filtered.</typeparam>
    internal interface IFilterable<out T, out TItem>
    {
        bool IsFiltered { get; }
        T GetFilteredView(Predicate<TItem> predicate);
    }
}
