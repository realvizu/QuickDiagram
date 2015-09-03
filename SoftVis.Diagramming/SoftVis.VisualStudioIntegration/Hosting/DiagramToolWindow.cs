using Codartis.SoftVis.VisualStudioIntegration.Commands;
using Codartis.SoftVis.VisualStudioIntegration.Presentation;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    [Guid("02d1f8b9-d0a0-4ccb-9687-e6f0f781ad9e")]
    public class DiagramToolWindow : ToolWindowPane
    {
        private readonly DiagramToolWindowControl _diagramToolWindowControl;

        public DiagramToolWindow() : base(null)
        {
            Caption = "Diagram";
            ToolBar = new CommandID(Constants.ToolWindowToolbarCommands, Constants.ToolWindowToolbar);
            _diagramToolWindowControl = new DiagramToolWindowControl();
            Content = _diagramToolWindowControl;
        }

        public double DiagramFontSize
        {
            get { return _diagramToolWindowControl.FontSize; }
            set { _diagramToolWindowControl.FontSize = value; }
        }

        public BitmapSource GetDiagramAsBitmap()
        {
            return _diagramToolWindowControl.GetDiagramAsBitmap();
        }
    }
}
