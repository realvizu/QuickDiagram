using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using Codartis.SoftVis.Modeling;
using Codartis.Util.UI.Wpf.Resources;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Converts a model entity stereotype to the Uri of its image.
    /// </summary>
    internal class StereotypeToImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stereotype = (ModelNodeStereotype)value;
            return ResourceHelpers.CreateUri(StereotypeToVisualPropertyMaps.StereotypeToImagePathMap[stereotype], Assembly.GetExecutingAssembly());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
