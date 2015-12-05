using System;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram shape is a representation of a model item on a diagram.
    /// </summary>
    public abstract class DiagramShape
    {
        public IModelItem ModelItem { get; }

        protected DiagramShape(IModelItem modelItem)
        {
            if (modelItem == null)
                throw new ArgumentNullException(nameof(modelItem));

            ModelItem = modelItem;
        }
    }
}
