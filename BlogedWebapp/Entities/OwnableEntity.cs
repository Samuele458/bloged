using BlogedWebapp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogedWebapp.Entities
{
    /// <summary>
    ///  Generic ownable entity. 
    /// </summary>
    /// <typeparam name="T">Owner type (Eg. AppUser)</typeparam>
    public class OwnableEntity<T> : BaseEntity
    {
        [Projection(ProjectionBehaviour.Preview)]
        public string OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        //[JsonIgnore]
        [Projection(ProjectionBehaviour.Full)]
        [RelatedEntity]
        public T Owner { get; set; }

    }

    /// <summary>
    ///  Entity owned by AppUser
    /// </summary>
    public class UserOwnableEntity : OwnableEntity<AppUser>
    {

    }


}
