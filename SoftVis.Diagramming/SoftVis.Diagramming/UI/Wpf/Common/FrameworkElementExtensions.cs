using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.Common
{
    public static class FrameworkElementExtensions
    {
        public static void AddResourceDictionary(this FrameworkElement frameworkElement, ResourceDictionary resourceDictionary)
        {
            frameworkElement.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        public static FrameworkElement FindParent<T>(this FrameworkElement frameworkElement)
            where T: FrameworkElement
        {
            var currentElement = frameworkElement;
            while (currentElement != null && !(currentElement is T))
                currentElement = VisualTreeHelper.GetParent(currentElement) as FrameworkElement;

            return currentElement;
        }

        public static IEnumerable<T> FindChildren<T>(this FrameworkElement frameworkElement, Predicate<T> predicate = null)
            where T : class
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(frameworkElement);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(frameworkElement, i) as FrameworkElement;

                var typedChild = child as T;
                if (typedChild != null && (predicate == null || predicate(typedChild)))
                    yield return typedChild;

                foreach (var foundChild in FindChildren(child, predicate))
                    yield return foundChild;
            }
        }
    }
}
