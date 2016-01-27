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
        public RelationshipSpecification RelationshipSpecification { get; }
        public Point AttachPoint { get; }
        public HandleOrientation HandleOrientation { get; }

        public DiagramButtonActivatedEventArgs(IModelEntity modelEntity, RelationshipSpecification relationshipSpecification,
            Point attachPoint, HandleOrientation handleOrientation)
        {
            ModelEntity = modelEntity;
            RelationshipSpecification = relationshipSpecification;
            AttachPoint = attachPoint;
            HandleOrientation = handleOrientation;
        }
    }
}
