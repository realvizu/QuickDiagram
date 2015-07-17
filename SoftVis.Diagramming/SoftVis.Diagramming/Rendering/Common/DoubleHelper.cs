using System;

namespace Codartis.SoftVis.Rendering.Common
{
    public static class DoubleHelper
    {
        private const double DoubleComparisonTolerance = .00001;

        public static bool EqualsWithTolerance(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= Math.Abs(value1 * DoubleComparisonTolerance);
        }
    }
}
