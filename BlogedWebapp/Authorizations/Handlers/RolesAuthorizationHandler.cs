using BlogedWebapp.Authorizations.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogedWebapp.Authorizations.Handlers
{
    public class RolesAuthorizationHandler : AuthorizationHandler<MinimumRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       MinimumRoleRequirement requirement)
        {

            // Getting user roles
            var roles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role);

            // Checking for superadmin role
            if (requirement.RoleName.Equals("Superadmin") &&
                roles.FirstOrDefault(c => c.Value.Equals("Superadmin")) != null)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Checking for admin role
            if (requirement.RoleName.Equals("Admin") &&
                (roles.FirstOrDefault(c => c.Value.Equals("Superadmin")) != null ||
                roles.FirstOrDefault(c => c.Value.Equals("Admin")) != null))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
