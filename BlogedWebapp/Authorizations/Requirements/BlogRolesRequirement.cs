using BlogedWebapp.Entities;
using Microsoft.AspNetCore.Authorization;

namespace BlogedWebapp.Authorizations.Requirements
{
    public class BlogRolesRequirement : IAuthorizationRequirement
    {

        private BlogRoles role;

        public BlogRolesRequirement(BlogRoles role)
        {
            this.role = role;
        }

        public BlogRoles Role
        {
            get { return this.role; }
        }

    }
}
