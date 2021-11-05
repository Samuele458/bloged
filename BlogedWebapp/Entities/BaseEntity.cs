using System;

namespace BlogedWebapp.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        public int Status { get; set; } = 1;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;


        public DateTime UpdatedOn { get; set; }
    }
}
