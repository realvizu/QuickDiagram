using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Codartis.SoftVis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// This control show a text at the current mouse position.
    /// </summary>
    public partial class PopupTextControl : UserControl
    {
        public PopupTextControl()
        {
            InitializeComponent();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue == false)
                return;

            var parentCanvas = Parent as Canvas;
            if (parentCanvas == null)
                return;

            var position = Mouse.GetPosition(parentCanvas);
            SetValue(Canvas.LeftProperty, position.X);
            SetValue(Canvas.TopProperty, position.Y);
        }
    }
}
