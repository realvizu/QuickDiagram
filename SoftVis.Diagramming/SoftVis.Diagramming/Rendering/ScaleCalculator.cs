using System;

namespace Codartis.SoftVis.Rendering
{
    internal static class ScaleCalculator
    {
        internal static double LinearToExponential(double linearValue, double minValue, double maxValue, double expBase)
        {
            var valueRange = maxValue - minValue;

            var normalizedLinearValue = (linearValue - minValue) / valueRange;
            var normalizedExponentialValue = (Math.Pow(expBase, normalizedLinearValue) - 1) / (expBase - 1);
            var exponentialValue = normalizedExponentialValue * valueRange + minValue;

            return exponentialValue;
        }

        internal static double ExponentialToLinear(double exponentialValue, double minValue, double maxValue, double expBase)
        {
            var valueRange = maxValue - minValue;

            var normalizedExponentialValue = (exponentialValue - minValue) / valueRange;
            var normalizedLinearValue = Math.Log(normalizedExponentialValue * (expBase - 1) + 1, expBase);
            var linearValue = normalizedLinearValue * valueRange + minValue;

            return linearValue;
        }
    }
}
