using System.Diagnostics;

namespace Codartis.SoftVis.UI.Wpf.Common
{
    [DebuggerDisplay("[{LowValue}-{HighValue}]")]
    public struct DoubleInterval
    {
        public double LowValue { get; }
        public double HighValue { get; }

        public DoubleInterval(double lowValue, double highValue)
        {
            LowValue = lowValue;
            HighValue = highValue;
        }
    }
}
