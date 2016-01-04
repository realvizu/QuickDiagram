using System;
using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.UI.Wpf.Common
{
    /// <summary>
    /// Abstract base class for custom templated controls.
    /// </summary>
    public abstract class TemplatedControlBase : Control
    {
        protected TUIElement FindChildControlInTemplate<TUIElement>(string controlName) where TUIElement : UIElement
        {
            var childControl = GetTemplateChild(controlName) as TUIElement;
            if (childControl == null)
                throw new Exception($"{controlName} control not found in the control template.");
            return childControl;
        }
    }
}