using System;

namespace Codartis.SoftVis.Graphs
{
    [Serializable]
    internal class PathException : Exception
    {
        public PathException(string message)
            :base(message)
        { }
    }
}