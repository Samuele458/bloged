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
    public class UserAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, User>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       SameAuthorRequirement requirement,
                                                       User resource)
        {
            System.Diagnostics.Debug.WriteLine("UserId" + context.User.Claims);

            
            context.Succeed(requirement);
            
            return Task.CompletedTask;
        }
    }
}
