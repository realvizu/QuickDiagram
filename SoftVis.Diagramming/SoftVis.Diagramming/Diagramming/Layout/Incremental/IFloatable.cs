namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Implementer's state can be set to either floating or non-floating (placed).
    /// </summary>
    /// <remarks>
    /// Floating diagram items don't use up space in the layout calculation.
    /// </remarks>
    internal interface IFloatable
    {
        bool IsFloating { get; }
    }
}
