using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Entities
{
    public class OwnableEntity : BaseEntity
    {

        //Profile id when logged in
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public IdentityUser User { get; set; }
    
    }
}

