using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public partial class CustomAuthProvider
{
    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public SecurityLevelRequirement(int level)
        {
            Level = level;
        }

        public int Level { get; set; }
    }

    public class SecurityLevelRequirementHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SecurityLevelRequirement requirement)
        {
            if (context.User.Claims.Any(x => x.Type == DynamicPolicies.SecurityLevel && requirement.Level <= (Convert.ToInt32(x.Value))))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}