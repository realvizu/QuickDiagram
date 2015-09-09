using System;

namespace Codartis.SoftVis.Rendering.Common
{
    /// <summary>
    /// Converts back-and-forth between a linear and an exponential scale.
    /// </summary>
    internal static class ScaleCalculator
    {
        private const double ExponentialScaleBase = 10d;

        internal static double LinearToExponential(double linearValue, double minValue, double maxValue)
        {
            var valueRange = maxValue - minValue;

            var normalizedLinearValue = (linearValue - minValue) / valueRange;
            var normalizedExponentialValue = (Math.Pow(ExponentialScaleBase, normalizedLinearValue) - 1) / (ExponentialScaleBase - 1);
            var exponentialValue = normalizedExponentialValue * valueRange + minValue;

            return exponentialValue;
        }

        internal static double ExponentialToLinear(double exponentialValue, double minValue, double maxValue)
        {
            var valueRange = maxValue - minValue;

            var normalizedExponentialValue = (exponentialValue - minValue) / valueRange;
            var normalizedLinearValue = Math.Log(normalizedExponentialValue * (ExponentialScaleBase - 1) + 1, ExponentialScaleBase);
            var linearValue = normalizedLinearValue * valueRange + minValue;

            return linearValue;
        }
    }
}
