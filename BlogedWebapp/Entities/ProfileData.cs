using BlogedWebapp.Helpers;

namespace BlogedWebapp.Entities
{
    /// <summary>
    ///  ProfileData entity
    /// </summary>
    public class ProfileData : UserOwnableEntity
    {
        [Projection(ProjectionBehaviour.Preview)]
        public string FirstName { get; set; }

        [Projection(ProjectionBehaviour.Preview)]
        public string LastName { get; set; }

    }
}
