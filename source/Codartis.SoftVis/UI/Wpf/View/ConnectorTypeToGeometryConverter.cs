using System;
using System.Globalization;
using System.Windows.Data;
using Codartis.SoftVis.Diagramming.Definition;
using WpfGeometry = System.Windows.Media.Geometry;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Converts a ConnectorType to its Geometry representation.
    /// </summary>
    internal class ConnectorTypeToGeometryConverter: IValueConverter
    {
        private const string SimpleArrowHead = "M -3 0 L 0 -5 L 3 0";
        private const string HollowArrowHead = "M -3 0 L 0 -5 L 3 0 Z";
        private const string SolidArrowShaft = "M 0 0 L 0 5";
        private const string DashedArrowShaft = "M 0 1 L 0 2.5 M 0 3.5 L 0 5";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ConnectorType))
                throw new ArgumentException($"{typeof(ConnectorType)} value expected.");

            var connectorType = (ConnectorType) value;

            var headGeometryString = GetHeadGeometryString(connectorType.ArrowHeadType);
            var shaftGeometryString = GetShaftGeometryString(connectorType.ShaftLineType);

            return CreateGeometry(headGeometryString, shaftGeometryString);
        }

        private static string GetHeadGeometryString(ArrowHeadType arrowHeadType)
        {
            switch (arrowHeadType)
            {
                case ArrowHeadType.Simple: return SimpleArrowHead;
                case ArrowHeadType.Hollow: return HollowArrowHead;
                default: throw new NotImplementedException($"{arrowHeadType} not implemented");
            }
        }

        private static string GetShaftGeometryString(LineType lineType)
        {
            switch (lineType)
            {
                case LineType.Solid: return SolidArrowShaft;
                case LineType.Dashed: return DashedArrowShaft;
                default: throw new NotImplementedException($"{lineType} not implemented");
            }
        }

        private static WpfGeometry CreateGeometry(params string[] geometryStrings)
        {
            return WpfGeometry.Parse(string.Join(" ", geometryStrings));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
