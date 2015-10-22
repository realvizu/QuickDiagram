namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// An action of a layout logic run.
    /// </summary>
    public interface ILayoutAction
    {
        string Action { get; }
        double? Amount { get; }
        string SubjectName { get; }
    }
}