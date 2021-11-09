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
        public Guid Id { get; set; }
    }

    /// <summary>
    ///  Base entity
    /// </summary>
    public abstract class BaseEntity : IBaseEntity, IIdentificableEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public int Status { get; set; } = 1;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedOn { get; set; }
    }
}
