using System;

namespace Codartis.SoftVis.UI.Common.UIEvents
{
    public class PanEventArgs : EventArgs
    {
        public PanDirection Direction { get; private set; }
        public double Amount { get; private set; }

        public PanEventArgs(PanDirection direction, double amount)
        {
            Direction = direction;
            Amount = amount;
        }
    }
}
