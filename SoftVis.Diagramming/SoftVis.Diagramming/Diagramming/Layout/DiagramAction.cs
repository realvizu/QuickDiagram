using System;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Abstract base class for all diagram actions.
    /// </summary>
    internal abstract class DiagramAction
    {
        public virtual void Accept(IDiagramActionVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
