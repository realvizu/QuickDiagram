using System;

namespace Codartis.SoftVis.Graphs
{
    [Serializable]
    public class PathException : Exception
    {
        public PathException(string message)
            :base(message)
        { }
    }
}