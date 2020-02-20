using System.Windows;

namespace Codartis.Util.UI.Wpf
{
    public static class FrameworkElementExtensions
    {
        public static Point GetCenterPoint(this FrameworkElement frameworkElement)
        {
            return new Point(frameworkElement.Width / 2, frameworkElement.Height / 2);
        }

        public static T FindDescendantByDataContext<T>(this FrameworkElement frameworkElement, object dataContext)
            where T : class
        {
            return frameworkElement.FindFirstDescendant<FrameworkElement>(i => i.DataContext == dataContext && i is T) as T;
        }

        public static void AddResourceDictionary(this FrameworkElement frameworkElement, ResourceDictionary resourceDictionary)
        {
            frameworkElement.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
