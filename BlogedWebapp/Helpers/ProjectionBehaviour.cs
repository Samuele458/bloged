namespace BlogedWebapp.Helpers
{
    /// <summary>
    ///  Projection behaviour for querying database
    /// </summary>
    public enum ProjectionBehaviour
    {
        /// <summary>
        ///  Gets just the most important and lightweight information
        /// </summary>
        Minimal,

        /// <summary>
        ///  Default projection behaviour
        ///  It does not remove fields, but also does not include related entities
        /// </summary>
        Default,

        /// <summary>
        ///  Includes related entities to projection
        /// </summary>
        IncludeRelated,

        /// <summary>
        ///  Include related entities to projection and recersively add
        ///  their related entities.
        /// </summary>
        IncludeRecursively
    }
}
