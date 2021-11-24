using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Authorizations.Requirements
{
    public class MinimumRoleRequirement : IAuthorizationRequirement
    {
        private string roleName;

        public MinimumRoleRequirement( string roleName )
        {
            this.roleName = roleName;
        }

        public string RoleName { 
            get { return this.roleName; } 
        }

    }
}
