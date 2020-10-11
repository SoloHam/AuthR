using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public partial class CustomAuthProvider : DefaultAuthorizationPolicyProvider
{
    private readonly ILogger<CustomAuthProvider> logger;

    public CustomAuthProvider(IOptions<AuthorizationOptions> options, ILogger<CustomAuthProvider> logger) : base(options)
    {
        this.logger = logger;
    }

    // {type}.{value}
    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        string[] policyParts = policyName.Split('.');
        logger.LogDebug(policyParts.Length.ToString());
        if (policyParts.Length == 2)
        {

            string type = policyParts[0];
            string value = policyParts[1];

            foreach (string policy in DynamicPolicies.Get())
            {
                if (policy == type)
                {
                    return DynamicPolicyProviderFactory.ResolvePolicy(type, value);
                }
            }
        }
        return await base.GetPolicyAsync(policyName);
    }


}