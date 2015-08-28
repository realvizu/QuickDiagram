using Codartis.SoftVis.VisualStudioIntegration.Presentation;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("02d1f8b9-d0a0-4ccb-9687-e6f0f781ad9e")]
    public class DiagramToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramToolWindow"/> class.
        /// </summary>
        public DiagramToolWindow() : base(null)
        {
            Caption = "Diagram";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Content = new DiagramToolWindowControl();
        }
    }
}
