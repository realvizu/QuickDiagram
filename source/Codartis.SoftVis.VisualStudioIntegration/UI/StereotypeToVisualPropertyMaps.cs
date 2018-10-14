using System.Collections.Generic;
using System.Windows.Media;
using Codartis.SoftVis.Modeling;
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

        public static readonly Dictionary<ModelNodeStereotype, string> StereotypeToImagePathMap =
            new Dictionary<ModelNodeStereotype, string>
            {
                {ModelNodeStereotypes.Class, string.Format(ImagePathTemplate, "Class")},
                {ModelNodeStereotypes.Interface, string.Format(ImagePathTemplate, "Interface")},
                {ModelNodeStereotypes.Struct, string.Format(ImagePathTemplate, "Struct")},
                {ModelNodeStereotypes.Enum, string.Format(ImagePathTemplate, "Enum")},
                {ModelNodeStereotypes.Delegate, string.Format(ImagePathTemplate, "Delegate")}
            };

        public static readonly Dictionary<ModelNodeStereotype, Brush> StereotypeToBackgroundBrushMap =
            new Dictionary<ModelNodeStereotype, Brush>
            {
                {ModelNodeStereotypes.Class, Color.FromArgb(0xFF, 0xF5, 0xE3, 0xD6).CreateBrushFrozen()},
                {ModelNodeStereotypes.Interface, Brushes.LightGray},
                {ModelNodeStereotypes.Struct, Color.FromArgb(0xFF, 0xD1, 0xEA, 0xF3).CreateBrushFrozen()},
                {ModelNodeStereotypes.Enum, Brushes.Gold},
                {ModelNodeStereotypes.Delegate, Brushes.Lavender}
            };
    }
}
