using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace BlogedWebapp.Entities
{
    [Owned]
    public  class RefreshTokens
    {

        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public bool IsExpired()
        {
            return DateTime.UtcNow > Expires;
        }

        public DateTime CreatedOn { get; set; }

        public String CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }

        public String RevokedByIp { get; set; }

        public String ReplacedByToken { get; set; }

        public bool IsActive()
        {
            return Revoked == null && !IsExpired();
        }
    }
}