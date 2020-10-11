using System;
using Microsoft.AspNetCore.Authorization;
using static CustomAuthProvider;

public class SecurityLevelAttribute : AuthorizeAttribute
{
    public SecurityLevelAttribute(int level)
    {
        Policy = DynamicPolicies.GetPolicyName(DynamicPolicies.SecurityLevel, level);
    }
}