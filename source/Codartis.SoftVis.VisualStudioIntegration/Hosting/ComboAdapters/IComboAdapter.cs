using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.ComboAdapters
{
    /// <summary>
    /// Interface for types that translate between a Selector and its implementation with VS commands.
    /// </summary>
    internal interface IComboAdapter
    {
        void GetItemsCommandHandler(object sender, EventArgs e);
        void ComboCommandHandler(object sender, EventArgs e);
    }
}
