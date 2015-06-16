using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    /// <summary>
    /// A gesture is a logic that turns input events (mouse, keyboard, control manipulation) 
    /// into graphical transforms (scale and translate).
    /// </summary>
    internal interface IGesture
    {
        event ScaleChangedEventHandler ScaleChanged;
        event TranslateChangedEventHandler TranslateChanged;

        IGestureTarget Target { get; }
    }
}
