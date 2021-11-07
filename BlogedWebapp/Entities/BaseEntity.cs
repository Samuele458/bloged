using System;
using System.ComponentModel.DataAnnotations;

namespace BlogedWebapp.Entities
{

    /// <summary>
    ///  Base entity
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public int Status { get; set; } = 1;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedOn { get; set; }
    }
}
