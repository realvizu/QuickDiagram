using System;

namespace Codartis.SoftVis.Graphs
{
    internal class PathException : Exception
    {
        public PathException(string message)
            :base(message)
        { }
    }
}