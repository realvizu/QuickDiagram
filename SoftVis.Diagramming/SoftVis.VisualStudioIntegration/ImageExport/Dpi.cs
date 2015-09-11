using System.Collections.Generic;

namespace Codartis.SoftVis.VisualStudioIntegration.ImageExport
{
    /// <summary>
    /// Describes a DPI (dots per inch) value.
    /// </summary>
    public struct Dpi
    {
        private static readonly Dpi Dpi96 = new Dpi(96, "96 dpi", "Low density screen");
        private static readonly Dpi Dpi150 = new Dpi(150, "150 dpi", "High density screen");
        private static readonly Dpi Dpi300 = new Dpi(300, "300 dpi", "Low quality print");
        private static readonly Dpi Dpi600 = new Dpi(600, "600 dpi", "Normal quality print");
        private static readonly Dpi Dpi1200 = new Dpi(1200, "1200 dpi", "High quality print");

        public static readonly IEnumerable<Dpi> DpiChoices = new List<Dpi> { Dpi96, Dpi150, Dpi300, Dpi600, Dpi1200 };
        public static readonly Dpi Default = Dpi150;

        public int Value { get; }
        public string Name { get; }
        public string Description { get; }

        private Dpi(int value, string name, string description)
        {
            Value = value;
            Name = name;
            Description = description;
        }
    }
}
