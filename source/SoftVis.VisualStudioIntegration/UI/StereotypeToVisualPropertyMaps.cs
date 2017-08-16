using System.Collections.Generic;
using System.Windows.Media;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Defines the visual property constants for the different model entity stereotypes (background brush, image uri).
    /// </summary>
    internal static class StereotypeToVisualPropertyMaps
    {
        private const string ImagePathTemplate = "/UI/Images/{0}.png";

        public static readonly Dictionary<NodeStereotype, string> StereotypeToImagePathMap =
            new Dictionary<NodeStereotype, string>
            {
                {NodeStereotype.Class, string.Format(ImagePathTemplate, "Class")},
                {NodeStereotype.Interface, string.Format(ImagePathTemplate, "Interface")},
                {NodeStereotype.Struct, string.Format(ImagePathTemplate, "Struct")},
                {NodeStereotype.Enum, string.Format(ImagePathTemplate, "Enum")},
                {NodeStereotype.Delegate, string.Format(ImagePathTemplate, "Delegate")}
            };

        public static readonly Dictionary<NodeStereotype, Brush> StereotypeToBackgroundBrushMap =
            new Dictionary<NodeStereotype, Brush>
            {
                {NodeStereotype.Class, Color.FromArgb(0xFF, 0xF5, 0xE3, 0xD6).CreateBrushFrozen()},
                {NodeStereotype.Interface, Brushes.LightGray},
                {NodeStereotype.Struct, Color.FromArgb(0xFF, 0xD1, 0xEA, 0xF3).CreateBrushFrozen()},
                {NodeStereotype.Enum, Brushes.Gold},
                {NodeStereotype.Delegate, Brushes.Lavender}
            };
    }
}
