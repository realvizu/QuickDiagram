using System;
using System.Globalization;
using System.Windows.Data;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Converts a model entity stereotype to the Uri of its image.
    /// </summary>
    internal class StereotypeToImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stereotype = value as RoslynBasedModelEntityStereotype;
            return stereotype == null 
                ? null 
                : WpfHelpers.CreateUri(StereotypeToVisualPropertyMaps.StereotypeToImagePathMap[stereotype]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
