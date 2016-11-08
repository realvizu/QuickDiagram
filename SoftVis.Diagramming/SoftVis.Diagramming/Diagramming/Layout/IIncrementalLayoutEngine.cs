using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Publishes layout action events.
    /// </summary>
    internal interface IIncrementalLayoutEngine
    {
        void Clear();
        IEnumerable<ILayoutAction> CalculateLayoutActions(IEnumerable<DiagramAction> diagramActions);
    }
}