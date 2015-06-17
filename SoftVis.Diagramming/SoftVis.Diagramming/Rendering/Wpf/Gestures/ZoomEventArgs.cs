using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    public class ZoomEventArgs : EventArgs
    {
        public double NewZoomValue { get; private set; }

        public ZoomEventArgs(double newZoomValue)
        {
            NewZoomValue = newZoomValue;
        }
    }
}
