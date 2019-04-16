using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.Util.UI.Wpf
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Converts the font properties of a Control to a Typeface object
        /// </summary>
        /// <param name="control">A control.</param>
        /// <returns>The typeface of the control's font.</returns>
        public static Typeface GetTypeface(this Control control)
            => new Typeface(control.FontFamily, control.FontStyle, control.FontWeight, control.FontStretch);
    }
}
