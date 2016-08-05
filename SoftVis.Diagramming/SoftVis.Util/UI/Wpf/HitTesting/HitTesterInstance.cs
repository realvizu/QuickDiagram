using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Codartis.SoftVis.Util.UI.Wpf.HitTesting
{
    /// <summary>
    /// Performs a hit test with a given root element and mouse event args.
    /// Returns the topmost UI element that IsVisible and IsHitTestVisble.
    /// The type of the searched UI element can be constrained using the generic method version.
    /// </summary>
    /// <remarks>
    /// A new instance must be created for each hit test to hold context info 
    /// because of the WPF hit testing API's structure.
    /// </remarks>
    internal sealed class HitTesterInstance
    {
        private UIElement _testingRoot;
        private UIElement _hitTestResult;
        private Type _typeFilter;

        public static UIElement HitTest(UIElement testingRoot, MouseEventArgs mouseEventArgs)
        {
            return new HitTesterInstance().HitTestPrivate(testingRoot, mouseEventArgs);
        }

        public static T HitTest<T>(UIElement testingRoot, MouseEventArgs mouseEventArgs)
            where T : UIElement
        {
            return (T)new HitTesterInstance().HitTestPrivate(testingRoot, mouseEventArgs, typeof(T));
        }

        private UIElement HitTestPrivate(UIElement testingRoot, MouseEventArgs mouseEventArgs, Type typeFilter = null)
        {
            _testingRoot = testingRoot;
            _hitTestResult = null;
            _typeFilter = typeFilter;

            var point = mouseEventArgs.GetPosition(_testingRoot);
            var pointHitTestParameters = new PointHitTestParameters(point);
            VisualTreeHelper.HitTest(_testingRoot, HitTestFilterCallback, HitTestResultCallback, pointHitTestParameters);

            return _hitTestResult;
        }

        private HitTestFilterBehavior HitTestFilterCallback(DependencyObject potentialHitTestTarget)
        {
            var uiElement = potentialHitTestTarget as UIElement;
            if (uiElement != null &&
                uiElement.IsVisible &&
                uiElement.IsHitTestVisible)
            {
                var result = _typeFilter != null && uiElement.GetType() == _typeFilter
                    ? HitTestFilterBehavior.ContinueSkipChildren
                    : HitTestFilterBehavior.Continue;
                return result;
            }
            return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        }

        private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            if (result?.VisualHit != null &&
                (_typeFilter == null || result.VisualHit.GetType() == _typeFilter))
            {
                _hitTestResult = (UIElement)result.VisualHit;
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }
    }
}
