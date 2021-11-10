using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Entities
{
    public class RefreshToken : BaseEntity
    {
        //User id when logged in
        public string UserId { get; set; }

        public string Token { get; set; }

        //Id that identifies a specific issued JWT
        public string JwtId { get; set; }

        //Every token must be used once
        public bool IsUsed { get; set; }

        //Make sude they are valid
        public bool IsRevoked { get; set; }

        public DateTime ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
