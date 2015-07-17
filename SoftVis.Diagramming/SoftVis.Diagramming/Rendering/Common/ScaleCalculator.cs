using System;

namespace Codartis.SoftVis.Rendering.Common
{
    internal static class ScaleCalculator
    {
        private const double _exponentialScaleBase = 10d;

        internal static double LinearToExponential(double linearValue, double minValue, double maxValue)
        {
            var valueRange = maxValue - minValue;

            var normalizedLinearValue = (linearValue - minValue) / valueRange;
            var normalizedExponentialValue = (Math.Pow(_exponentialScaleBase, normalizedLinearValue) - 1) / (_exponentialScaleBase - 1);
            var exponentialValue = normalizedExponentialValue * valueRange + minValue;

            return exponentialValue;
        }

        internal static double ExponentialToLinear(double exponentialValue, double minValue, double maxValue)
        {
            var valueRange = maxValue - minValue;

            var normalizedExponentialValue = (exponentialValue - minValue) / valueRange;
            var normalizedLinearValue = Math.Log(normalizedExponentialValue * (_exponentialScaleBase - 1) + 1, _exponentialScaleBase);
            var linearValue = normalizedLinearValue * valueRange + minValue;

            return linearValue;
        }
    }
}
