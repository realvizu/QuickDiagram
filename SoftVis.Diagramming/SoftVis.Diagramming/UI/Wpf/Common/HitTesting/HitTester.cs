using System.Windows;
using System.Windows.Input;

namespace Codartis.SoftVis.UI.Wpf.Common.HitTesting
{
    /// <summary>
    /// Performs hit tests for a given root element.
    /// Returns the topmost UI element that IsVisible and IsHitTestVisble.
    /// </summary>
    /// <remarks>
    /// Creates a new hit tester instance for each hit test.
    /// </remarks>
    internal sealed class HitTester : IHitTester
    {
        private readonly UIElement _testingRoot;

        public HitTester(UIElement testingRoot)
        {
            _testingRoot = testingRoot;
        }

        public UIElement HitTest(MouseEventArgs mouseEventArgs)
        {
            return HitTesterInstance.HitTest(_testingRoot, mouseEventArgs);
        }

        public T HitTest<T>(MouseEventArgs mouseEventArgs)
            where T : UIElement
        {
            return HitTesterInstance.HitTest<T>(_testingRoot, mouseEventArgs);
        }
    }
}
