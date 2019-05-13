using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Converts diagram actions to layout actions.
    /// Stateful.
    /// </summary>
    internal interface IIncrementalLayoutCalculator
    {
        void Clear();
        IEnumerable<ILayoutAction> CalculateLayoutActions(IEnumerable<DiagramAction> diagramActions);
    }
}