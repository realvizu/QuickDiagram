using System;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Incremental
{
    [Serializable]
    internal class LayoutPathException : Exception
    {
        public LayoutPathException(string message) : base(message)
        { }
    }
}