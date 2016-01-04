using System;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Geometry;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;
using Codartis.SoftVis.UI.Wpf.DiagramRendering.Shapes;
using WpfGeometry = System.Windows.Media.Geometry;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Modification.MiniButtons
{
    /// <summary>
    /// A button on diagram nodes for showing related entities.
    /// </summary>
    internal class ShowRelatedEntityMiniButton : MiniButtonBase
    {
        private static readonly WpfGeometry SimpleArrowHeadGeometry = WpfGeometry.Parse("M -3 0 L 0 -5 L 3 0");
        private static readonly WpfGeometry HollowArrowHeadGeometry = WpfGeometry.Parse("M -3 0 L 0 -5 L 3 0 Z");
        private static readonly WpfGeometry SimpleArrowShaftGeometry = WpfGeometry.Parse("M 0 0 L 0 5");
        private static readonly WpfGeometry DashedArrowShaftGeometry = WpfGeometry.Parse("M 0 1 L 0 2.5 M 0 3.5 L 0 5");

        private readonly RelatedEntityMiniButtonDescriptor _miniButtonDescriptor;

        public ShowRelatedEntityMiniButton(RelatedEntityMiniButtonDescriptor miniButtonDescriptor,
            DiagramNodeControl adornedDiagramNodeControl, Visibility initialVisibility = Visibility.Visible)
            : base(adornedDiagramNodeControl, initialVisibility)
        {
            _miniButtonDescriptor = miniButtonDescriptor;
        }

        public DiagramNodeControl AdornedDiagramNodeControl => (DiagramNodeControl) AdornedShape;
        public RelationshipSpecification RelationshipSpecification => _miniButtonDescriptor.RelationshipSpecification;
        public RectRelativeLocation MiniButtonLocation => _miniButtonDescriptor.MiniButtonLocation;

        public Rect GetRect(Point adornedElementTopLeft)
        {
            var centerToTopLeftVector = new Vector(MiniButtonRadius, MiniButtonRadius);
            var location = GetButtonCenterRelativeToAdornedControl() - centerToTopLeftVector + (Vector)adornedElementTopLeft;
            var size = new Size(MiniButtonRadius*2, MiniButtonRadius*2);
            return new Rect(location, size);
        }

        protected override Point GetButtonCenterRelativeToAdornedControl()
        {
            return new Rect(AdornedElement.DesiredSize).GetRelativePoint(_miniButtonDescriptor.MiniButtonLocation);
        }

        protected override void DrawPicture(DrawingContext drawingContext, Point center)
        {
            var pen = new Pen(AdornedShape.Foreground, 1);
            var brush = AdornedShape.Background;
            var geometry = GetConnectorGeometry(_miniButtonDescriptor.ConnectorStyle, center);
            drawingContext.DrawGeometry(brush, pen, geometry);
        }

        private static WpfGeometry GetConnectorGeometry(ConnectorType connectorType, Point center)
        {
            var translateTransform = new TranslateTransform(center.X, center.Y);

            var arrowHeadGeometry = CreateArrowHeadGeometry(connectorType.ArrowHeadType);
            var arrowShaftGeometry = CreateArrowShaftGeometry(connectorType.ShaftLineType);

            var geometry = new PathGeometry { Transform = translateTransform };
            geometry.AddGeometry(arrowHeadGeometry);
            geometry.AddGeometry(arrowShaftGeometry);
            return geometry;
        }

        private static WpfGeometry CreateArrowHeadGeometry(ArrowHeadType arrowHeadType)
        {
            switch (arrowHeadType)
            {
                case ArrowHeadType.Simple:
                    return SimpleArrowHeadGeometry;
                case ArrowHeadType.Hollow:
                    return HollowArrowHeadGeometry;
                default:
                    throw new Exception($"Unexpected ArrowHeadType: {arrowHeadType}");
            }
        }

        private static WpfGeometry CreateArrowShaftGeometry(LineType lineType)
        {
            switch (lineType)
            {
                case LineType.Solid:
                    return SimpleArrowShaftGeometry;
                case LineType.Dashed:
                    return DashedArrowShaftGeometry;
                default:
                    throw new Exception($"Unexpected LineType: {lineType}");
            }
        }
    }
}
