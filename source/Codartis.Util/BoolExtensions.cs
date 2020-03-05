using System;

namespace Codartis.Util
{
    public static class BoolExtensions
    {
        /// <summary>
        /// The logical implies operator.
        /// </summary>
        public static bool Implies(this bool p, Func<bool> q)
        {
            return !p || q();
        }
    }
}