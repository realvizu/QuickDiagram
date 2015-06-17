using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    public class ScaleSpecification
    {
        public double Scale { get; private set; }
        public Point Center { get; private set; }

        public ScaleSpecification(double scale, Point center)
        {
            Scale = scale;
            Center = center;
        }
    }
}
