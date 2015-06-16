using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    internal class ScaleChangedEventArgs : EventArgs
    {
        public double NewScale { get; private set; }

        public ScaleChangedEventArgs(double newScale)
        {
            NewScale = newScale;
        }
    }
}
