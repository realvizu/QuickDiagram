using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    /// <summary>
    /// A gesture target is a UI element that display graphics that can be transformed with gestures. 
    /// </summary>
    internal interface IGestureTarget : IInputElement, IAnimatable
    {
        Vector Translate { get; }
        double Scale { get; }
        double ActualWidth { get; }
        double ActualHeight { get; }
        Cursor Cursor { get; set; }
    }
}
