using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public class CustomRequirement : IAuthorizationRequirement
{
    public CustomRequirement(string claimType)
    {
        ClaimType = claimType;
    }

    public string ClaimType { get; set; }
}

public class CustomRequirementHandler : AuthorizationHandler<CustomRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirement requirement)
    {
        var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
        if (hasClaim)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireCustomClaim(this AuthorizationPolicyBuilder builder, string claimType)
    {
        builder.AddRequirements(new CustomRequirement(claimType));
        return builder;
    }
}