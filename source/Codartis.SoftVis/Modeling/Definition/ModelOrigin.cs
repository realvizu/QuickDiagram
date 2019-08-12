namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Enumerates the possible sources of model information.
    /// TODO: change it to arbitrary tags? To separate the generic model from source code related concepts?
    /// </summary>
    public enum ModelOrigin
    {
        Unknown,
        SourceCode,
        Metadata
    }
}
