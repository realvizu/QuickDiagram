using System;
using System.Windows;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Event data for MiniButtonActivated event.
    /// </summary>
    internal class MiniButtonActivatedEventArgs : EventArgs
    {
        public IModelEntity ModelEntity { get; }
        public RelationshipSpecification RelationshipSpecification { get; }
        public Point AttachPoint { get; }
        public HandleOrientation HandleOrientation { get; }

        public MiniButtonActivatedEventArgs(IModelEntity modelEntity, RelationshipSpecification relationshipSpecification,
            Point attachPoint, HandleOrientation handleOrientation)
        {
            ModelEntity = modelEntity;
            RelationshipSpecification = relationshipSpecification;
            AttachPoint = attachPoint;
            HandleOrientation = handleOrientation;
        }
    }
}
