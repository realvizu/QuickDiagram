using System;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Shapes
{
    /// <summary>
    /// A diagram shape is a representation of a model element on a diagram.
    /// </summary>
    public abstract class DiagramShape
    {
        public UmlModelElement ModelElement { get; private set; }

        protected DiagramShape(UmlModelElement modelElement)
        {
            if (modelElement == null) throw new ArgumentNullException(nameof(modelElement));

            ModelElement = modelElement;
        }
    }
}
