namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// An action performed by the layout logic.
    /// </summary>
    public interface ILayoutAction
    {
        string Action { get; }
        double? Amount { get; }
        ILayoutAction CausingLayoutAction { get; }
        string SubjectName { get; }
    }
}