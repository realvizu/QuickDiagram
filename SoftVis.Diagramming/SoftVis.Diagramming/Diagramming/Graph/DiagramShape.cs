using System;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Graph
{
    /// <summary>
    /// A diagram shape is a representation of a model item on a diagram.
    /// </summary>
    public abstract class DiagramShape
    {
        protected IModelItem ModelItem { get; }

        protected DiagramShape(IModelItem modelItem)
        {
            if (modelItem == null)
                throw new ArgumentNullException(nameof(modelItem));

            ModelItem = modelItem;
        }
    }
}
