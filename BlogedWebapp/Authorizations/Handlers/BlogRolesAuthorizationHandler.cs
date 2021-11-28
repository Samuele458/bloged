using BlogedWebapp.Authorizations.Requirements;
using BlogedWebapp.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogedWebapp.Authorizations.Handlers
{
    public class BlogRolesAuthorizationHandler : AuthorizationHandler<BlogRolesRequirement, Blog>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       BlogRolesRequirement requirement,
                                                       Blog resource)
        {

            // Getting user roles
            var roles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role);

            // Checking for admin roles
            if (roles.FirstOrDefault(c => c.Value.Equals("Superadmin")) != null ||
                roles.FirstOrDefault(c => c.Value.Equals("Admin")) != null)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Getting identityId from claims
            var identityId = context.User.Claims.FirstOrDefault(c => c.Type.Equals("Id")).Value;

            // Searching for user
            var usersBlogItem = resource
                                    .UsersBlog
                                    .FirstOrDefault(u => u.OwnerId.Equals(identityId));

            if (usersBlogItem != null &&
                usersBlogItem.Role >= requirement.Role)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
