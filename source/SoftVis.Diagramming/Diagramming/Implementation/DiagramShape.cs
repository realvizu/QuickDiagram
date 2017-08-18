using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// The common base type for all diagram shapes (nodes and connectors as well).
    /// </summary>
    public abstract class DiagramShape : IDiagramShape
    {
        public IModelItem ModelItem { get; private set; }

        protected DiagramShape(IModelItem modelItem)
        {
            ModelItem = modelItem ?? throw new ArgumentNullException(nameof(modelItem));
        }

        public abstract bool IsRectDefined { get; }
        public abstract Rect2D Rect { get; }

        public virtual void Update(IModelItem modelItem)
        {
            if (modelItem == null)
                throw new ArgumentNullException(nameof(modelItem));

            if (modelItem.Id != ModelItem.Id)
                throw new InvalidOperationException($"ModelItem Id mistmatch when updating DiagramShape. Old:{ModelItem.Id} New:{modelItem.Id}");

            ModelItem = modelItem;
        }
    }
}
