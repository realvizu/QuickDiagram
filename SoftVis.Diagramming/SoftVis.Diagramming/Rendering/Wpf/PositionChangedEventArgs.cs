using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf
{
    public class PositionChangedEventArgs : RoutedEventArgs
    {
        public double XChange { get; private set; }
        public double YChange { get; private set; }

        public PositionChangedEventArgs(RoutedEvent evt, object source, double xChange, double yChange)
            : base(evt, source)
        {
            XChange = xChange;
            YChange = yChange;
        }
    }
}
