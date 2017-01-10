using System;

namespace Codartis.SoftVis.Util
{
    public static class DoubleHelper
    {
        private const double DoubleComparisonTolerance = .00001;

        public static bool IsEqualWithTolerance(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= Math.Abs(value1 * DoubleComparisonTolerance);
        }

        public static bool IsUndefined(this double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }

        public static bool IsDefined(this double value) => !value.IsUndefined();
    }
}
