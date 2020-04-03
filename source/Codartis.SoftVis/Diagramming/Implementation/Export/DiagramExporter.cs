using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Export;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Codartis.SoftVis.Diagramming.Implementation.Export
{
    public sealed class DiagramExporter : IDiagramExporter
    {
        public async Task ExportAsync(IDiagram diagram, string path)
        {
            var diagramSpec = CreateDiagramSpec(diagram);
            var json = JsonConvert.SerializeObject(diagramSpec);
            await WriteToFileAsync(path, json);
        }

        private static async Task WriteToFileAsync([NotNull] string path, [NotNull] string content)
        {
            using (var streamWriter = new StreamWriter(path))
            {
                await streamWriter.WriteAsync(content);
            }
        }

        private static DiagramSpec CreateDiagramSpec([NotNull] IDiagram diagram)
        {
            return new DiagramSpec
            {
                Nodes = diagram.Nodes.Select(ToNodeSpec).ToArray(),
                Connector = diagram.Connectors.Select(ToConnectorSpec).ToArray()
            };
        }

        private static NodeSpec ToNodeSpec([NotNull] IDiagramNode node)
        {
            return new NodeSpec
            {
                Id = (long)node.Id,
                Name = node.Name,
                Stereotype = node.ModelNode.Stereotype.Name,
                ParentNodeId = node.ParentNodeId.HasValue ? (long)node.ParentNodeId.Value : (long?)null,
                AddedAt = node.AddedAt,
                Center = node.AbsoluteRect.Center,
                Size = node.Size,
                ChildrenAreaSize = node.ChildrenAreaSize
            };
        }

        private static ConnectorSpec ToConnectorSpec([NotNull] IDiagramConnector connector)
        {
            return new ConnectorSpec
            {
                Id = (long)connector.Id,
                SourceNodeId = (long)connector.Source,
                TargetNodeId = (long)connector.Target,
                Stereotype = connector.ModelRelationship.Stereotype.Name,
                Route = connector.Route
            };
        }
    }
}