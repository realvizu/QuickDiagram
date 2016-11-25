namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Describes a DPI (dots per inch) value.
    /// </summary>
    public struct Dpi
    {
        public static readonly Dpi Dpi96 = new Dpi(96, "96 dpi", "Low resolution screen");
        public static readonly Dpi Dpi150 = new Dpi(150, "150 dpi", "High resolution screen");
        public static readonly Dpi Dpi300 = new Dpi(300, "300 dpi", "Low quality print");
        public static readonly Dpi Dpi600 = new Dpi(600, "600 dpi", "High quality print");
        public static readonly Dpi Dpi1200 = new Dpi(1200, "1200 dpi", "High quality print");

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
