namespace Codartis.SoftVis.Services
{
    /// <summary>
    /// Identifies diagram plugins.
    /// </summary>
    public class DiagramPluginId
    {
        public static readonly DiagramPluginId AutoLayoutDiagramPlugin = new DiagramPluginId(nameof(AutoLayoutDiagramPlugin));
        public static readonly DiagramPluginId ModelTrackingDiagramPlugin = new DiagramPluginId(nameof(ModelTrackingDiagramPlugin));
        public static readonly DiagramPluginId ConnectorHandlerDiagramPlugin = new DiagramPluginId(nameof(ConnectorHandlerDiagramPlugin));

        public string Name { get; }

        public DiagramPluginId(string name)
        {
            Name = name;
        }
    }
}
