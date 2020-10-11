using System;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

public partial class CustomAuthProvider
{
    public static class DynamicPolicies
    {
        public static IEnumerable Get()
        {
            yield return SecurityLevel;
        }

        internal static string GetPolicyName(string type, int value) => $"{type}.{value}";

        public const string SecurityLevel = "SecurityLevel";
    }
    public static class DynamicPolicyProviderFactory
    {
        public static AuthorizationPolicy ResolvePolicy(string type, string value)
        {
            switch (type)
            {
                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                    .AddRequirements(new SecurityLevelRequirement(Convert.ToInt32(value)))
                    // .RequireClaim(type, new[] { value })
                    .Build();
            }

            return null;
        }
    }

}