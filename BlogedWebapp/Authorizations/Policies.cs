using BlogedWebapp.Authorizations.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;

namespace BlogedWebapp.Authorizations
{
    /// <summary>
    ///  Static class for getting authorization policies
    /// </summary>
    public static class Policies
    {
        public static Action<AuthorizationPolicyBuilder> AllowedToUse()
        {
            Action<AuthorizationPolicyBuilder> policyBuilder = options =>
            {
                /*options
                    .AuthenticationSchemes
                    .Add(JwtBearerDefaults.AuthenticationScheme);
                */

                options
                    .RequireAuthenticatedUser();

                options
                    .Requirements
                    .Add(new OwnerRequirement());
            };

            return policyBuilder;
        }

        public static Action<AuthorizationPolicyBuilder> AdminOrSuperadmin()
        {
            Action<AuthorizationPolicyBuilder> policyBuilder = options =>
            {
                options
                    .RequireAuthenticatedUser();

                options
                    .Requirements
                    .Add(new MinimumRoleRequirement("Admin"));
            };

            return policyBuilder;
        }

        public static Action<AuthorizationPolicyBuilder> BlogOwner()
        {
            Action<AuthorizationPolicyBuilder> policyBuilder = options =>
            {
                /*options
                    .AuthenticationSchemes
                    .Add(JwtBearerDefaults.AuthenticationScheme);
                */

                options
                    .RequireAuthenticatedUser();

                options
                    .Requirements
                    .Add(new BlogRolesRequirement(Entities.BlogRoles.Owner));
            };

            return policyBuilder;
        }

        public static Action<AuthorizationPolicyBuilder> AtLeastBlogWriter()
        {
            Action<AuthorizationPolicyBuilder> policyBuilder = options =>
            {
                /*options
                    .AuthenticationSchemes
                    .Add(JwtBearerDefaults.AuthenticationScheme);
                */

                options
                    .RequireAuthenticatedUser();

                options
                    .Requirements
                    .Add(new BlogRolesRequirement(Entities.BlogRoles.Writer));
            };

            return policyBuilder;
        }
    }
}
