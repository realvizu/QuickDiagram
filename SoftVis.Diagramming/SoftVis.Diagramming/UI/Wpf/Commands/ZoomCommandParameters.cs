using System.Windows;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    internal class ZoomCommandParameters
    {
        public ZoomDirection Direction { get; }
        public double Amount { get; }
        public Point Center { get; }

        public ZoomCommandParameters(ZoomDirection direction, double amount, Point center)
        {
            Direction = direction;
            Amount = amount;
            Center = center;
        }
    }
}
