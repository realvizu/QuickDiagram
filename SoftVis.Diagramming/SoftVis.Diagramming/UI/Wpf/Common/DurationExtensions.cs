using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.Common
{
    internal static class DurationExtensions
    {
        internal static Duration ToDuration(this int millisec)
        {
            return new Duration(new TimeSpan(0, 0, 0, 0, millisec));
        }
    }
}
