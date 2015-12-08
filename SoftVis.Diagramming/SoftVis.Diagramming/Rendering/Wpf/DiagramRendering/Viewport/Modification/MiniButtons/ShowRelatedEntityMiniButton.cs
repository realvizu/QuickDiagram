using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Extensibility;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Modification.MiniButtons
{
    /// <summary>
    /// A button on diagram nodes for showing related entities.
    /// </summary>
    internal class ShowRelatedEntityMiniButton : MiniButtonBase
    {
        private readonly RelatedEntityMiniButtonDescriptor _miniButtonDescriptor;

        public ShowRelatedEntityMiniButton(RelatedEntityMiniButtonDescriptor miniButtonDescriptor,
            Control adornedControl, Visibility initialVisibility = Visibility.Collapsed)
            : base(adornedControl, initialVisibility)
        {
            _miniButtonDescriptor = miniButtonDescriptor;
        }

        protected override Point GetButtonCenter()
        {
            return new Rect(AdornedElement.DesiredSize).GetRelativePoint(_miniButtonDescriptor.MiniButtonLocation);
        }

        protected override void DrawPicture(DrawingContext drawingContext, Point center)
        {
            var pen = new Pen(AdornedControl.Foreground, 1);
            var brush = AdornedControl.Background;
            var geometry = GetConnectorGeometry(_miniButtonDescriptor.ConnectorStyle);
            //drawingContext.DrawGeometry(brush, pen, geometry);
        }

        private System.Windows.Media.Geometry GetConnectorGeometry(ConnectorType connectorType)
        {
            // TODO
            return null;
        }
    }
}
