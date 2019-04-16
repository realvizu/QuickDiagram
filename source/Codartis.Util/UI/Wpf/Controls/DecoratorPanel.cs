using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// Arranges its child controls based on a placement specification that is relative to its parent control rectangle.
    /// </summary>
    /// <remarks>
    /// The children must define its desired placement either by the Placement or the PlacementKey attached property.
    /// PlacementKey identifies an entry in the PlacementDictionary property.
    /// If PlacementKey is used then PlacementDictionary must also be set.
    /// If both Placement and PlacementKey are defined for a child control then Placement takes precedence.
    /// </remarks>
    public class DecoratorPanel : Panel
    {
        /// <summary>
        /// Attached property used by the panel's child element to define the location of its center point
        /// relative to the panel's parent control.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached("Placement", typeof(RectRelativePlacement), typeof(DecoratorPanel),
                new FrameworkPropertyMetadata(RectRelativePlacement.Undefined,
                    FrameworkPropertyMetadataOptions.Inherits |
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Attached property used by the panel's child element to define the location of its center point
        /// relative to the panel's parent control by supplying a key 
        /// that identifies an entry in the PlacementDictionary property.
        /// </summary>
        public static readonly DependencyProperty PlacementKeyProperty =
            DependencyProperty.RegisterAttached("PlacementKey", typeof(object), typeof(DecoratorPanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// A dictionary of placement definitions. The child controls can use the PlacementKey attached property
        /// to identify a certain entry in this dictionary to be used as their desired placement.
        /// </summary>
        public static readonly DependencyProperty PlacementDictionaryProperty =
            DependencyProperty.Register("PlacementDictionary", typeof(IDictionary), typeof(DecoratorPanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        public static void SetPlacement(UIElement element, RectRelativePlacement value) => element.SetValue(PlacementProperty, value);
        public static RectRelativePlacement GetPlacement(UIElement element) => (RectRelativePlacement)element.GetValue(PlacementProperty);

        public static void SetPlacementKey(UIElement element, object value) => element.SetValue(PlacementKeyProperty, value);
        public static object GetPlacementKey(UIElement element) => element.GetValue(PlacementKeyProperty);

        public IDictionary PlacementDictionary
        {
            get { return (IDictionary)GetValue(PlacementDictionaryProperty); }
            set { SetValue(PlacementDictionaryProperty, value); }
        }

        /// <summary>
        /// Measures the child elements.
        /// </summary>
        /// <param name="constraint"> The size limit that should not be exceeded.</param>
        /// <returns>The size that is required to arrange child content.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            foreach (UIElement internalChild in InternalChildren)
                internalChild?.Measure(constraint);

            return Size.Empty;
        }

        /// <summary>
        /// Arranges the contents.
        /// </summary>
        /// <param name="arrangeSize">The size that this panel should use to arrange its child elements.</param>
        /// <returns>The arranged size of this panel and its descendants.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            ArrangeChildrenRelativeToRect(new Rect(arrangeSize));

            return arrangeSize;
        }

        protected void ArrangeChildrenRelativeToRect(Rect decoratedRect)
        {
            foreach (UIElement internalChild in InternalChildren)
            {
                if (internalChild == null)
                    continue;

                var childPlacement = GetChildPlacement(internalChild);
                if (childPlacement == null)
                    continue;

                var childSize = internalChild.DesiredSize;
                var childPosition = CalculateChildPosition(decoratedRect, childPlacement.Value, childSize);

                internalChild.Arrange(new Rect(childPosition, internalChild.DesiredSize));
            }
        }

        private RectRelativePlacement? GetChildPlacement(UIElement uiElement)
        {
            var explicitPlacement = GetPlacement(uiElement);
            if (explicitPlacement != RectRelativePlacement.Undefined)
                return explicitPlacement;

            var placementKey = GetPlacementKey(uiElement);
            if (PlacementDictionary == null || !PlacementDictionary.Contains(placementKey))
                return null;

            var dictionaryValue = PlacementDictionary[placementKey];
            if (dictionaryValue.GetType() != typeof(RectRelativePlacement))
                return null;

            var rectRelativePlacement = (RectRelativePlacement)dictionaryValue;
            SetPlacement(uiElement, rectRelativePlacement);

            return rectRelativePlacement;
        }

        private static Point CalculateChildPosition(Rect decoratedRect, RectRelativePlacement childPlacement, Size childSize)
        {
            var childCenterPointRelativeToParent = childPlacement.GetPositionRelativeTo(decoratedRect);
            var childCenterToTopLeftVector = new Vector(-childSize.Width / 2, -childSize.Height / 2);
            var childTopLeftPointRelativeToParent = childCenterPointRelativeToParent + childCenterToTopLeftVector;
            return childTopLeftPointRelativeToParent;
        }
    }
}
