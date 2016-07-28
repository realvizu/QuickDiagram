using System.Collections.Generic;
using System.Windows.Media;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Defines the visual property constants for the different model entity stereotypes (background brush, image uri).
    /// </summary>
    internal static class StereotypeToVisualPropertyMaps 
    {
        private const string ImagePathTemplate = "/UI/Images/{0}.png";

        public static readonly Dictionary<ModelEntityStereotype, string> StereotypeToImagePathMap =
            new Dictionary<ModelEntityStereotype, string>
            {
                { ModelEntityStereotypes.Class, string.Format(ImagePathTemplate, "Class")},
                { ModelEntityStereotypes.Interface, string.Format(ImagePathTemplate, "Interface")},
                { ModelEntityStereotypes.Struct, string.Format(ImagePathTemplate, "Struct")},
                { ModelEntityStereotypes.Enum, string.Format(ImagePathTemplate, "Enum")},
                { ModelEntityStereotypes.Delegate, string.Format(ImagePathTemplate, "Delegate")}
            };

        public static readonly Dictionary<ModelEntityStereotype, Brush> StereotypeToBackgroundBrushMap =
            new Dictionary<ModelEntityStereotype, Brush>
            {
                { ModelEntityStereotypes.Class, new SolidColorBrush(Color.FromArgb(0xFF, 0xF5, 0xE3, 0xD6))},
                { ModelEntityStereotypes.Interface, Brushes.LightGray},
                { ModelEntityStereotypes.Struct, new SolidColorBrush(Color.FromArgb(0xFF, 0xD1, 0xEA, 0xF3))},
                { ModelEntityStereotypes.Enum, Brushes.Gold},
                { ModelEntityStereotypes.Delegate, Brushes.Lavender}
            };
    }
}
