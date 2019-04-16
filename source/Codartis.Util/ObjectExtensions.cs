using System.Linq;

namespace Codartis.Util
{
    public static class ObjectExtensions
    {
        public static bool In(this object o, params object[] others)
        {
            return others.Any(o.Equals);
        }
    }
}
