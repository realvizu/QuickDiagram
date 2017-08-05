namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// Enumerates the possible sources of model information.
    /// </summary>
    public enum ModelOrigin
    {
        /// <summary>
        /// The origin of the model information is not known.
        /// </summary>
        Unknown,

        /// <summary>
        /// The model information comes from source code.
        /// </summary>
        SourceCode,

        /// <summary>
        /// The model information comes from metadata.
        /// </summary>
        Metadata
    }
}
