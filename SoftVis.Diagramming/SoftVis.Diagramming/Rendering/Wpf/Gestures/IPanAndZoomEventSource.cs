using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    internal interface IPanAndZoomEventSource
    {
        event PanEventHandler Pan;
        event ZoomEventHandler Zoom;
    }
}
