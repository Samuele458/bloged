using BlogedWebapp.Authorizations.Requirements;
using BlogedWebapp.Entities;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogedWebapp.Authorizations.Handlers
{
    public class OwnerAuthorizationHandler : AuthorizationHandler<OwnerRequirement, OwnableEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OwnerRequirement requirement,
                                                       OwnableEntity resource)
        {
            
            // Getting user roles
            var roles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role);

            // Checking for admin role
            if (roles.FirstOrDefault(c => c.Value.Equals("Superadmin")) != null ||
                roles.FirstOrDefault(c => c.Value.Equals("Admin")) != null)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            
            // Getting identityId from claims
            var identityId = context.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"));

            // Checking if IDs are equals
            if(identityId != null && resource.User.Id.Equals(identityId.Value) )
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
