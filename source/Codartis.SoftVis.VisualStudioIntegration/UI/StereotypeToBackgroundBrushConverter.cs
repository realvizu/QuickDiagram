using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Converts a model entity stereotype to its background brush.
    /// </summary>
    internal class StereotypeToBackgroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stereotype = (ModelNodeStereotype)value;

            return StereotypeToVisualPropertyMaps.StereotypeToBackgroundBrushMap.TryGetValue(stereotype, out var brush)
                ? brush
                : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}