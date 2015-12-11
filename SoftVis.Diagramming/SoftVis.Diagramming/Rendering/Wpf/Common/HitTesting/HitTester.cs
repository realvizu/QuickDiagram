using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Codartis.SoftVis.Rendering.Wpf.Common.HitTesting
{
    /// <summary>
    /// Hit testing logic with a given test root.
    /// The test starts from the test root and the topmost UI element is returned 
    /// whose IsVisible and IsHitTestVisble are true.
    /// </summary>
    internal sealed class HitTester : IHitTester
    {
        private readonly UIElement _testingRoot;
        private UIElement _hitTestResult;

        public HitTester(UIElement testingRoot)
        {
            _testingRoot = testingRoot;
        }

        public UIElement HitTest(MouseEventArgs mouseEventArgs)
        {
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
