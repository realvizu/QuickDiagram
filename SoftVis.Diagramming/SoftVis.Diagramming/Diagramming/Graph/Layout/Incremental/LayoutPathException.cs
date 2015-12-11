using System;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.Incremental
{
    [Serializable]
    internal class LayoutPathException : Exception
    {
        public LayoutPathException(string message) : base(message)
        { }
    }
}