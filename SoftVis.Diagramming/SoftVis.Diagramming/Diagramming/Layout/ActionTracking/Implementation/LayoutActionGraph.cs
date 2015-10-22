using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using QuickGraph;
using QuickGraph.Serialization;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation
{
    /// <summary>
    /// Represents the actions of a layout logic run and the causality relationship between actions.
    /// </summary>
    internal class LayoutActionGraph : BidirectionalGraph<ILayoutAction, ILayoutActionEdge>, ILayoutActionGraph
    {
        public LayoutActionGraph()
            : base(false)
        { }

        public int VertexMoveCount => Vertices.OfType<IVertexMoveAction>().Count();

        public string Serialize()
        {
            using (var memoryStream = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8 };

                using (var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    var serializer = new GraphMLSerializer<ILayoutAction, ILayoutActionEdge, LayoutActionGraph>();
                    serializer.Serialize(xmlWriter, this, v => v.ToString(), e => e.ToString());
                    xmlWriter.Flush();
                }

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public void Save(string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.Write(Serialize());
            }
        }
    }
}
