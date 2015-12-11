using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Codartis.SoftVis.Rendering.Wpf.Common.HitTesting
{
    /// <summary>
    /// Performs a hit test with a given root element and mouse event args.
    /// Returns the topmost UI element that IsVisible and IsHitTestVisble.
    /// </summary>
    /// <remarks>
    /// A new instance must be created for each hit test to hold context info 
    /// because of the WPF hit testing API's structure.
    /// </remarks>
    internal sealed class HitTesterInstance
    {
        private UIElement _testingRoot;
        private UIElement _hitTestResult;

        public static UIElement HitTest(UIElement testingRoot, MouseEventArgs mouseEventArgs)
        {
            return new HitTesterInstance().HitTestPrivate(testingRoot, mouseEventArgs);
        }

        private UIElement HitTestPrivate(UIElement testingRoot, MouseEventArgs mouseEventArgs)
        {
            _testingRoot = testingRoot;
            _hitTestResult = null;

            var point = mouseEventArgs.GetPosition(_testingRoot);

            VisualTreeHelper.HitTest(_testingRoot, HitTestFilterCallback, HitTestResultCallback,
                new PointHitTestParameters(point));

            return _hitTestResult;
        }

        private static HitTestFilterBehavior HitTestFilterCallback(DependencyObject potentialHitTestTarget)
        {
            var uiElement = potentialHitTestTarget as UIElement;
            if (uiElement != null &&
                uiElement.IsVisible &&
                uiElement.IsHitTestVisible)
                return HitTestFilterBehavior.Continue;

            return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        }

        private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            if (result?.VisualHit is UIElement)
                _hitTestResult = (UIElement)result.VisualHit;

            return HitTestResultBehavior.Stop;
        }
    }
}
