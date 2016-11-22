using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.Util.UI.Wpf
{
    public static class FormattedTextHelpers
    {
        /// <summary>
        /// Returns the size of a text formatted with a given font specified by a control.
        /// </summary>
        /// <param name="text">A text.</param>
        /// <param name="fontReference">A control that supplies font properties.</param>
        /// <returns>The size of the text formatted with the control's font.</returns>
        public static Size Measure(string text, Control fontReference)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;

            var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                fontReference.GetTypeface(), fontReference.FontSize, Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
