using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.Util.UI.Wpf
{
    public static class DependencyObjectExtensions
    {
        public static T FindAncestor<T>(this DependencyObject dependencyObject)
            where T : class
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent == null || parent is T)
                return parent as T;

            return FindAncestor<T>(parent);
        }

        public static IEnumerable<T> FindDescendants<T>(this DependencyObject dependencyObject, Predicate<T> predicate = null)
            where T : class
        {
            var children = new List<DependencyObject>();

            var childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                if (child is T childAsT && (predicate == null || predicate(childAsT)))
                    yield return childAsT;

                children.Add(child);
            }

            foreach (var child in children)
                foreach (var foundChild in FindDescendants(child, predicate))
                    yield return foundChild;
        }

        public static T FindFirstDescendant<T>(this DependencyObject dependencyObject, Predicate<T> predicate = null)
            where T : class 
            => FindDescendants(dependencyObject, predicate).FirstOrDefault();
    }
}
