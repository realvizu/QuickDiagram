using System;

namespace Codartis.SoftVis.Rendering.Common.UIEvents
{
    public class ResizeEventArgs : EventArgs
    {
        public double NewWidth { get; private set; }
        public double NewHeight { get; private set; }

        public ResizeEventArgs(double newWidth, double newHeight)
        {
            NewHeight = newHeight;
            NewWidth = newWidth;
        }
    }
}
