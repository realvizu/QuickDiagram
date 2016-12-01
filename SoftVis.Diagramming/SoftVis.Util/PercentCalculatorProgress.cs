using System;

namespace Codartis.SoftVis.Util
{
    /// <summary>
    /// A progress reporting class that is initialized with a total number, is called with the increments (from 0 to total)
    /// and calculates the actual/total percent for each call to Report.
    /// </summary>
    public class PercentCalculatorProgress : Progress<double>
    {
        private readonly int _total;
        private double _actual;

        public PercentCalculatorProgress(Action<double> handler, int total)
            : base(handler)
        {
            if (total <= 0)
                throw new ArgumentException("Total must be greater than 0.", nameof(total));

            _total = total;
            _actual = 0;
        }

        protected override void OnReport(double value)
        {
            _actual += value;
            base.OnReport(_actual/_total);
        }
    }
}
