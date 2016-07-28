using System;
using System.Windows;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Event data for diagram button activation.
    /// </summary>
    internal class DiagramButtonActivatedEventArgs : EventArgs
    {
        public IModelEntity ModelEntity { get; }
        public RelatedEntitySpecification RelatedEntitySpecification { get; }
        public Point AttachPoint { get; }
        public HandleOrientation HandleOrientation { get; }

        public DiagramButtonActivatedEventArgs(IModelEntity modelEntity, RelatedEntitySpecification relatedEntitySpecification,
            Point attachPoint, HandleOrientation handleOrientation)
        {
            ModelEntity = modelEntity;
            RelatedEntitySpecification = relatedEntitySpecification;
            AttachPoint = attachPoint;
            HandleOrientation = handleOrientation;
        }
    }
}
