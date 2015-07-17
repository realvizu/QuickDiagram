using System;

namespace Codartis.SoftVis.Rendering.Common.UIEvents
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
