using System;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama
{
    [Serializable]
    internal class LayoutPathException : Exception
    {
        public LayoutPathException(string message) : base(message)
        { }
    }
}