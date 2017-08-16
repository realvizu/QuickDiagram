using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using Codartis.SoftVis.Util.UI.Wpf.Resources;
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
            var stereotype = (NodeStereotype)value;
            return ResourceHelpers.CreateUri(StereotypeToVisualPropertyMaps.StereotypeToImagePathMap[stereotype], Assembly.GetExecutingAssembly());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
