
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OAuth;

public class HomeController : Controller
{
    private readonly IConfiguration configuration;

    public HomeController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> Authenticate()
    {
        // var opts = configuration.GetSection("OAuthOptions") as CustomJwtBearerOptions;

        var SecretBytes = Encoding.UTF8.GetBytes(CustomJwtBearerOptions.Secret);
        var key = new SymmetricSecurityKey(SecretBytes);

        var token = new JwtSecurityToken(CustomJwtBearerOptions.Issuer, CustomJwtBearerOptions.Audience,
            new List<Claim>(){
                new Claim(JwtRegisteredClaimNames.Sub, "x")
            }
            , DateTime.Now, DateTime.Now.AddHours(1), new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

        return await Task.FromResult(Ok(new { access_token = tokenJson }));
    }

    [Authorize]
    public IActionResult Secret()
    {
        return Content("Jeeeyry");
    }
}