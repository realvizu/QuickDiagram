using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.Common
{
    public static class UIElementExtensions
    {
        public static T FindParent<T>(this UIElement uiElement)
            where T: UIElement
        {
            var currentElement = uiElement;
            while (currentElement != null && !(currentElement is T))
                currentElement = VisualTreeHelper.GetParent(currentElement) as UIElement;

            return (T)currentElement;
        }

        public static IEnumerable<T> FindChildren<T>(this UIElement uiElement, Predicate<T> predicate = null)
            where T : class
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(uiElement);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(uiElement, i) as UIElement;

                var typedChild = child as T;
                if (typedChild != null && (predicate == null || predicate(typedChild)))
                    yield return typedChild;

                foreach (var foundChild in FindChildren(child, predicate))
                    yield return foundChild;
            }
        }
    }
}
