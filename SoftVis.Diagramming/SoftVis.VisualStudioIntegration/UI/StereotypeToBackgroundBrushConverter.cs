using System;
using System.Globalization;
using System.Windows.Data;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Converts a model entity stereotype to its background brush.
    /// </summary>
    internal class StereotypeToBackgroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stereotype = value as RoslynBasedModelEntityStereotype;
            return stereotype == null 
                ? null 
                : StereotypeToVisualPropertyMaps.StereotypeToBackgroundBrushMap[stereotype];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
