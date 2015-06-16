using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    public class PanEventArgs : EventArgs
    {
        public PanDirection Direction { get; private set; }

        public PanEventArgs(PanDirection direction)
        {
            Direction = direction;
        }
    }
}
