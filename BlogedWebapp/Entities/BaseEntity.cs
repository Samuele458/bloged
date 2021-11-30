using System;
using System.ComponentModel.DataAnnotations;

namespace BlogedWebapp.Entities
{

    public interface IBaseEntity
    {

        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }

    public interface IIdentificableEntity
    {
        public string Id { get; set; }
    }

    /// <summary>
    ///  Base entity
    /// </summary>
    public abstract class BaseEntity : IBaseEntity, IIdentificableEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int Status { get; set; } = 1;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedOn { get; set; }
    }
}
