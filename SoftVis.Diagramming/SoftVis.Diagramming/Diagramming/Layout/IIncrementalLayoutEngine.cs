using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Converts diagram actions to layout actions.
    /// </summary>
    internal interface IIncrementalLayoutEngine
    {
        void Clear();
        IEnumerable<ILayoutAction> CalculateLayoutActions(IEnumerable<DiagramAction> diagramActions);
    }
}